using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class DitheringRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class DitheringSettings
        {
            public Material ditheringMaterial = null;
        }

        public DitheringSettings settings = new DitheringSettings();

        class DitheringPass : ScriptableRenderPass
        {
            public Material material;
            private RTHandle tempTexture;

            public DitheringPass(Material mat)
            {
                material = mat;
            }

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                tempTexture = RTHandles.Alloc(cameraTextureDescriptor, name: "_DitheringTemp");
                ConfigureTarget(tempTexture);
                ConfigureClear(ClearFlag.None, Color.clear);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get("DitheringPass");

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

        DitheringPass ditheringPass;

        public override void Create()
        {
            if (settings.ditheringMaterial != null)
            {
                ditheringPass = new DitheringPass(settings.ditheringMaterial);
                ditheringPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (ditheringPass != null)
            {
                renderer.EnqueuePass(ditheringPass);
            }
        }
    }
}
