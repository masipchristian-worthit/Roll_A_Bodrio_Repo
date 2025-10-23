using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class PSXLowResRendererFeature : ScriptableRendererFeature
{
    class PSXLowResPass : ScriptableRenderPass
    {
        private Material blitMaterial;
        private RTHandle lowResRT;
        private RTHandle cameraColorHandle;
        private int width, height;
        private string profilerTag = "PSXLowResPass";

        public PSXLowResPass(Material material, int w, int h)
        {
            blitMaterial = material;
            width = w;
            height = h;
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

            // Crear RT temporal low-res
            if (lowResRT == null)
            {
                var desc = renderingData.cameraData.cameraTargetDescriptor;
                desc.width = width;
                desc.height = height;
                desc.msaaSamples = 1;
                lowResRT = RTHandles.Alloc(desc, name: "LowResBuffer");
            }

            // Blit de cámara a RT pequeño
            Blitter.BlitCameraTexture(cmd,
                cameraColorHandle,
                lowResRT,
                blitMaterial,
                0);

            // Blit de RT pequeño de vuelta a cámara
            Blitter.BlitCameraTexture(cmd,
                lowResRT,
                cameraColorHandle,
                blitMaterial,
                0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (lowResRT != null)
            {
                RTHandles.Release(lowResRT);
                lowResRT = null;
            }
        }
    }

    [System.Serializable]
    public class PSXSettings
    {
        public Material blitMaterial;
        public int lowResWidth = 320;
        public int lowResHeight = 240;
    }

    public PSXSettings settings = new PSXSettings();
    PSXLowResPass lowResPass;

    public override void Create()
    {
        lowResPass = new PSXLowResPass(settings.blitMaterial, settings.lowResWidth, settings.lowResHeight);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.blitMaterial != null)
        {
            // PASO CLAVE: pasar cameraColorTargetHandle dentro del scope del Render Pass
            lowResPass.Setup(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(lowResPass);
        }
    }
}
