using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        private PixelationPass pixelationPass;

        public override void Create()
        {
            pixelationPass = new PixelationPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pixelationPass);
        }

        class PixelationPass : ScriptableRenderPass
        {
            private const string ShaderPath = "PostEffect/Pixelation";
            private const string ProfilerTag = "Render Pixelation Effect";

            private static readonly int TempTargetId = Shader.PropertyToID("_TempTargetPixelation");
            private static readonly int WidthPixelation = Shader.PropertyToID("_WidthPixelation");
            private static readonly int HeightPixelation = Shader.PropertyToID("_HeightPixelation");
            private static readonly int ColorPrecision = Shader.PropertyToID("_ColorPrecision");

            private Material material;

            public PixelationPass(RenderPassEvent evt = RenderPassEvent.BeforeRenderingPostProcessing)
            {
                renderPassEvent = evt;
                var shader = Shader.Find(ShaderPath);
                if (shader == null) Debug.LogError($"Shader not found: {ShaderPath}");
                material = CoreUtils.CreateEngineMaterial(shader);
            }

            [System.Obsolete]
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (!renderingData.cameraData.postProcessEnabled || material == null) return;

                var stack = VolumeManager.instance.stack;
                var settings = stack.GetComponent<Pixelation>();
                if (settings == null || !settings.IsActive()) return;

                var cmd = CommandBufferPool.Get(ProfilerTag);

#pragma warning disable CS0618
                var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
#pragma warning restore CS0618

                int w = renderingData.cameraData.camera.scaledPixelWidth;
                int h = renderingData.cameraData.camera.scaledPixelHeight;

                material.SetFloat(WidthPixelation, settings.widthPixelation.value);
                material.SetFloat(HeightPixelation, settings.heightPixelation.value);
                material.SetFloat(ColorPrecision, settings.colorPrecision.value);

                cmd.GetTemporaryRT(TempTargetId, w, h, 0, FilterMode.Point);
                cmd.Blit(source, TempTargetId);
                cmd.Blit(TempTargetId, source, material);
                cmd.ReleaseTemporaryRT(TempTargetId);

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
    }
}


