using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class PixelationSettings
        {
            public Material pixelationMaterial = null;
        }

        public PixelationSettings settings = new PixelationSettings();

        class PixelationPass : ScriptableRenderPass
        {
            public Material material;
            private RTHandle tempTexture;

            public PixelationPass(Material mat)
            {
                material = mat;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                tempTexture = RTHandles.Alloc(cameraTextureDescriptor, name: "_PixelationTemp");
                ConfigureTarget(tempTexture);
                ConfigureClear(ClearFlag.None, Color.clear);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("PixelationPass");

                var renderer = renderingData.cameraData.renderer;
                var cameraColor = renderer.cameraColorTargetHandle;

                // Blit de cámara a temp con material
                Blit(cmd, cameraColor, tempTexture, material, 0);
                // Blit de temp de vuelta a la cámara
                Blit(cmd, tempTexture, cameraColor);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                if (tempTexture != null)
                {
                    RTHandles.Release(tempTexture);
                    tempTexture = null;
                }
            }
        }

        PixelationPass pixelationPass;

        public override void Create()
        {
            if (settings.pixelationMaterial != null)
            {
                pixelationPass = new PixelationPass(settings.pixelationMaterial);
                pixelationPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (pixelationPass != null)
            {
                renderer.EnqueuePass(pixelationPass);
            }
        }
    }
}
