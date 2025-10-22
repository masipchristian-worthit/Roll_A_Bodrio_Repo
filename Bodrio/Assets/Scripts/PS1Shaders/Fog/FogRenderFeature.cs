using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class FogSettings
        {
            public Material material = null;
        }

        public FogSettings settings = new FogSettings();
        FogPass pass;

        public override void Create()
        {
            pass = new FogPass(settings.material);
            pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            pass.Setup(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(pass);
        }

        class FogPass : ScriptableRenderPass
        {
            Material material;
            RTHandle cameraColorTargetRT;

            public FogPass(Material mat)
            {
                material = mat;
            }

            public void Setup(RTHandle target)
            {
                cameraColorTargetRT = target;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (material == null) return;

                CommandBuffer cmd = CommandBufferPool.Get("FogPass");

                material.SetVector("_ScreenParams", new Vector4(cameraColorTargetRT.rt.width, cameraColorTargetRT.rt.height, 0, 0));
                cmd.SetGlobalTexture("_MainTex", cameraColorTargetRT);

                Blit(cmd, cameraColorTargetRT, cameraColorTargetRT, material);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }
}
