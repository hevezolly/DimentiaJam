using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SanityEffect : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material blurMaterial = null;
    }

    public Settings settings = new Settings();

    class CustomRenderPass : ScriptableRenderPass
    {
        public Material effectMaterial;

        int tmpId1;

        RenderTargetIdentifier tmpRT1;

        public CustomRenderPass(string profilerTag)
        {
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = cameraTextureDescriptor.width;
            var height = cameraTextureDescriptor.height;

            tmpId1 = Shader.PropertyToID("tmpBlurRT1");
            cmd.GetTemporaryRT(tmpId1, width, height, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);

            tmpRT1 = new RenderTargetIdentifier(tmpId1);

            ConfigureTarget(tmpRT1);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            // first pass
            // cmd.GetTemporaryRT(tmpId1, opaqueDesc, FilterMode.Bilinear);

            cmd.Blit(renderingData.cameraData.targetTexture, tmpRT1, effectMaterial);
            cmd.Blit(tmpRT1, renderingData.cameraData.targetTexture);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass scriptablePass;

    public override void Create()
    {
        scriptablePass = new CustomRenderPass("KawaseBlur");
        scriptablePass.effectMaterial = settings.blurMaterial;

        scriptablePass.renderPassEvent = settings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(scriptablePass);
    }
}