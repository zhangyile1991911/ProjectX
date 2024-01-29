Shader "Vertex Animation/Flip Page/UI"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _BackTex ("Back",2D) = "white" {}
        _Radius ("Radius",float) = 5
        _DragPoint("Point",Vector) = (0,0,0,0)
        _Direction("Direction",Vector) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "UnLit"
            "RenderType"="Transparent"
        }
        
        CGINCLUDE
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            #include "UnityCG.cginc"
            
            float4x4 _Canvas2Local;
            float4x4 _Local2Canvas;
            
            float _Radius;
            float2 _DragPoint;
            float2 _Direction;
            
            
            v2f vert (appdata v)
            {
                v2f o;
                //将顶点转换到当前画布局部坐标
                v.vertex = mul(_Canvas2Local,v.vertex);

                //转换后的坐标点
                float3 p = v.vertex.xyz;
                float2 dir = normalize(_Direction);

                //当前点 指向_Point 向量 在 方向上的投影距离
                //假设Direction -1,1,1
                float dist = dot((_DragPoint - p.xy),dir.xy);
                if(dist > 0)
                {
                    //沿着轴方向移动
                    float2 bottom = p.xy + dist * dir.xy;
                    //UNITY_PI * _Radius周长的一半
                    float moreThanHalfCircle = (dist - UNITY_PI * _Radius);
                    //当前的点 已经超过了一半的圆周长 说明不用旋转了,因为 当前坐标 已经被
                    if(moreThanHalfCircle >= 0)
                    {
                        //底部的坐标+直径
                        float3 topPoint = float3(bottom,2*_Radius);
                        p = topPoint + moreThanHalfCircle * float3(dir.xy,0);
                    }
                    else
                    {
                        float angle = UNITY_PI - dist/_Radius;
                        float h = dist - sin(angle) * _Radius;
                        float z = _Radius + cos(angle) * _Radius;
                        float3 vD = p + h * float3(dir.xy,0);
                        p = float3(vD.xy,z);
                    }
                    p.z = -p.z;
                }
                v.vertex.xyz = p;
                v.vertex = mul(_Local2Canvas,v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

             
        ENDCG

       Pass
        {
            Name "MultiPass0"
            Tags
            {
                "LightMode"="MultiPass0"
            }
            Cull Front
            Offset -1,-1 //对深度值进行偏移，用于避免Z-fighting（深度冲突）的问题。
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragBack
            
            uniform sampler2D _BackTex;
            fixed4 fragBack(v2f i) : SV_Target
            {
                float2 invertUV = float2(1.0f-i.uv.x,i.uv.y);
                float4 col = tex2D(_BackTex,invertUV);
                return col;
            }
            ENDCG
        }

        Pass
        {
            Name "MultiPass1"
            Tags
            {
                "LightMode"="MultiPass1"
            }
            Cull Back
            Offset -1,-1 //对深度值进行偏移，用于避免Z-fighting（深度冲突）的问题。
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragFront
            uniform sampler2D _MainTex;
            fixed4 fragFront(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex,i.uv);
                return col;
            }
            ENDCG
        }
    }

}