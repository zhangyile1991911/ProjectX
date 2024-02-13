Shader "Unlit/cookProgress"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        _MaskTex ("_MaskTex",2D) = "white" {}
        _low("_low",float) = 0.3
        _medium("_medium",float) = 0.6
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Cull Off
		Lighting Off
		ZWrite Off
        
        Stencil {
            Ref 1
            Comp equal
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                // float2 mask_uv: TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                // float2 mask_uv: TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MainTex_ST;
            // float4 _MaskTex_ST;
            float _low;
            float _medium;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // o.mask_uv = TRANSFORM_TEX(v.mask_uv,_MaskTex);
                // o.mask_uv = v.mask_uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.y = step(i.uv.x,_low) * 0.1f;
                i.uv.y += (1 - step(i.uv.x, _low)) * (1 - step(_medium, i.uv.x))*0.4;
                i.uv.y += step(_medium,i.uv.x)*0.7f;
                
                fixed4 col = tex2D(_MainTex, i.uv);
                // fixed4 maskcol = tex2D(_MaskTex,i.uv);

                // clip(maskcol.a - 0.001);
                return col;    
            }
            ENDCG
        }
    }
}
