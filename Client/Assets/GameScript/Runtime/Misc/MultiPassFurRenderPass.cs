using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MultiPassFurRenderPass : ScriptableRenderPass
{
    private const int MultiPassCount = 8;
    private const string LightModeTag = "MultiPass";
    private ShaderTagId[] tagIdList = null;

    private readonly RenderPassEvent _renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    private readonly RenderQueueRange _renderQueueRange = RenderQueueRange.transparent;

    public MultiPassFurRenderPass()
    {
        renderPassEvent = _renderPassEvent;
        tagIdList = new ShaderTagId[MultiPassCount + 1];
        for (int i = 0; i < MultiPassCount+1; i++)
        {
            var name = LightModeTag + i.ToString();
            tagIdList[i] = new ShaderTagId(name);
        }
    }

    public override void Execute(ScriptableRenderContext context,ref RenderingData renderingData)
    {
        for (var i = 0; i < MultiPassCount + 1; i++)
        {
            var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
            var dSettings = CreateDrawingSettings(tagIdList[i], ref renderingData, sortFlags);
            var fSetting = new FilteringSettings(_renderQueueRange, -1);
            dSettings.perObjectData = PerObjectData.None;
            context.DrawRenderers(renderingData.cullResults,ref dSettings,ref fSetting);
        }
    }
}