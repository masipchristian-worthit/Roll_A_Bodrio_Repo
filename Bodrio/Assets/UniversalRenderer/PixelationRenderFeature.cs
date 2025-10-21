using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        private PixelationPass pass;

        public override void Create()
        {
            pass = new PixelationPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }

        private class PixelationPass : ScriptableRenderPass
        {
            private Material material;
            private const string ShaderPath = "PostEffect/Pixelation";

            private static readonly int TempTargetId = Shader.PropertyToID("_TempPixelation");
            private static readonly int WidthPixelation = Shader.PropertyToID("_WidthPixelation");
            private static readonly int HeightPixelation = Shader.PropertyToID("_HeightPixelation");
            private static readonly int ColorPrecision = Shader.PropertyToID("_ColorPrecision");

            public PixelationPass(RenderPassEvent evt)
            {
                renderPassEvent = evt;
                Shader shader = Shader.Find(ShaderPath);
                if (shader != null)
                    material = CoreUtils.CreateEngineMaterial(shader);
                else
                    Debug.LogError($"Shader not found: {ShaderPath}");
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ref RenderingData renderingData)
            {
                if (material == null || !renderingData.cameraData.postProcessEnabled) return;

                var stack = VolumeManager.instance.stack;
                var settings = stack.GetComponent<Pixelation>();
                if (settings == null || !settings.IsActive()) return;

                var colorTarget = renderingData.cameraData.renderer.cameraColorTarget;

                renderGraph.AddRenderPass<PassData>("Pixelation", out var passData, colorTarget)
                    .SetRenderFunc((PassData data, RenderGraphContext ctx) =>
                    {
                        var cmd = ctx.cmd;
                        int w = renderingData.cameraData.camera.scaledPixelWidth;
                        int h = renderingData.cameraData.camera.scaledPixelHeight;

                        cmd.GetTemporaryRT(TempTargetId, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);

                        material.SetFloat(WidthPixelation, settings.widthPixelation.value);
                        material.SetFloat(HeightPixelation, settings.heightPixelation.value);
                        material.SetFloat(ColorPrecision, settings.colorPrecision.value);

                        cmd.Blit(colorTarget, TempTargetId);
                        cmd.Blit(TempTargetId, colorTarget, material);
                        cmd.ReleaseTemporaryRT(TempTargetId);
                    });
            }

            private class PassData { }
        }
    }
}
