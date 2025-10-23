using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class PSXRetroRendererFeature : ScriptableRendererFeature
{
    class PSXRetroPass : ScriptableRenderPass
    {
        private Material blitMaterial;
        private RTHandle cameraColorHandle;
        private string profilerTag = "PSXRetroPass";

        public PSXRetroPass(Material material)
        {
            blitMaterial = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void Setup(RTHandle cameraColorTarget)
        {
            cameraColorHandle = cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (blitMaterial == null || cameraColorHandle == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

            // Aplicar shader PSX a toda la cámara
            Blitter.BlitCameraTexture(cmd,
                cameraColorHandle,
                cameraColorHandle,
                blitMaterial,
                0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    [System.Serializable]
    public class PSXSettings
    {
        public Material blitMaterial;
    }

    public PSXSettings settings = new PSXSettings();
    PSXRetroPass retroPass;

    public override void Create()
    {
        retroPass = new PSXRetroPass(settings.blitMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.blitMaterial != null)
        {
            // PASO CLAVE: pasar cameraColorTargetHandle dentro del scope del Render Pass
            retroPass.Setup(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(retroPass);
        }
    }
}
