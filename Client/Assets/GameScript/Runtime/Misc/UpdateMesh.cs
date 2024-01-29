using System;
using UnityEngine;
using UnityEngine.UI;

public class UpdateMesh : MonoBehaviour
{
    private Canvas _canvas;
    private Material _material;
    
    private void Start()
    {
        var image = GetComponent<Image>();
        _material = image.material;
    }

    private void Update()
    {
        UpdateMatrix();
    }

    void UpdateMatrix() 
    {
        var matrix = CalcCanvas2LocalMatrix();
        
        _material.SetMatrix("_Canvas2Local", matrix);
        _material.SetMatrix("_Local2Canvas", matrix.inverse);
    }
    
    public Matrix4x4 CalcCanvas2LocalMatrix()
    {
        if (_canvas == null)
        {
            var canvaslist = GetComponentsInParent<Canvas>();
            int parentIndex = canvaslist.Length - 1;
            _canvas = canvaslist[parentIndex];
            
        }
        return transform.worldToLocalMatrix * _canvas.transform.localToWorldMatrix;
    }
}