using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class InterpolateMesh : BaseMeshEffect
{
    [SerializeField,Range(0,6)]
    public int Level = 3;
    public override void ModifyMesh(VertexHelper vh)
    {
        var existedMesh = ListPool<UIVertex>.Get();
        vh.GetUIVertexStream(existedMesh);
        var gridMesh = new List<UIVertex>();
        for (int i = 0; i < existedMesh.Count; i += 3)
        {
            gridMesh.AddRange(interpolatePoint(existedMesh[i],existedMesh[i+1],existedMesh[i+2],Level));
        }

        ListPool<UIVertex>.Release(existedMesh);;
        vh.Clear();
        vh.AddUIVertexTriangleStream(gridMesh);
    }

    //利用递归,继续分割网格
    List<UIVertex> interpolatePoint(UIVertex a,UIVertex b,UIVertex c,int depth)
    {
        if (depth == 0)
            return new List<UIVertex>(){a,b,c};
        var curList = insertMiddlePoint(a, b, c);
        List<UIVertex> aggregateList = new List<UIVertex>();
        for (int i = 0; i < curList.Count; i += 3)
            aggregateList.AddRange(interpolatePoint(curList[i], curList[i + 1], curList[i + 2], depth - 1));
        return aggregateList;
    }

    List<UIVertex> insertMiddlePoint(UIVertex a,UIVertex b,UIVertex c)
    {
        //在一个三角形中 切割出四个
        /*
         * b    c   b    bc   c
         *          ab     ca 
         * a        a
         */
        
        UIVertex middleOfAB = LerpUIVertex(a,b,0.5f);
        UIVertex middleOfBC = LerpUIVertex(b, c,0.5f);
        UIVertex middleOfCA = LerpUIVertex(c, a,0.5f);
        return new List<UIVertex>()
        {
            a,middleOfAB,middleOfCA,
            middleOfAB,middleOfBC,middleOfCA,
            middleOfAB,b,middleOfBC,
            middleOfCA,middleOfBC,c
        };
    }

    UIVertex LerpUIVertex(UIVertex a,UIVertex b,float percent)
    {
        UIVertex one = new UIVertex();
        one.position = Vector3.Lerp(a.position, b.position, percent);
        one.normal = Vector3.Lerp(a.normal, b.normal, percent);
        one.tangent = Vector3.Lerp(a.tangent, b.tangent, percent);
        one.color = Color32.Lerp(a.color,b.color,percent);
        one.uv0 = Vector2.Lerp(a.uv0, b.uv0,percent);
        one.uv1 = Vector2.Lerp(a.uv1, b.uv1,percent);
        one.uv2 = Vector2.Lerp(a.uv2, b.uv2,percent);
        one.uv3 = Vector2.Lerp(a.uv3, b.uv3,percent);
        return one;
    }
}