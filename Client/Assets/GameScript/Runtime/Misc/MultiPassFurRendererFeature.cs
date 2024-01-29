using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MultiPassFurRendererFeature : ScriptableRendererFeature
{
    private MultiPassFurRenderPass _renderPass = null;

    public override void Create()
    {
        _renderPass = new MultiPassFurRenderPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_renderPass == null)
        {
            return;
        }
        renderer.EnqueuePass(_renderPass);
    }
}