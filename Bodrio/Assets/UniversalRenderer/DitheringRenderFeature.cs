using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class DitheringRenderFeature : ScriptableRendererFeature
    {
        private DitheringPass pass;

        public override void Create()
        {
            pass = new DitheringPass(RenderPassEvent.AfterRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }
    }

    public class DitheringPass : ScriptableRenderPass
    {
        private Material material;
        private const string ShaderPath = "PostEffect/Dithering";

        private static readonly int TempTargetId = Shader.PropertyToID("_TempDithering");
        private static readonly int PatternIndex = Shader.PropertyToID("_PatternIndex");
        private static readonly int DitherThreshold = Shader.PropertyToID("_DitherThreshold");
        private static readonly int DitherStrength = Shader.PropertyToID("_DitherStrength");
        private static readonly int DitherScale = Shader.PropertyToID("_DitherScale");

        public DitheringPass(RenderPassEvent evt)
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
            if (material == null || !renderingData.cameraData.postProcessEnabled)
                return;

            var stack = VolumeManager.instance.stack;
            var settings = stack.GetComponent<Dithering>();
            if (settings == null || !settings.IsActive()) return;

            var colorTarget = renderingData.cameraData.renderer.cameraColorTarget;

            renderGraph.AddRenderPass<PassData>("Dithering", out var passData, colorTarget)
                .SetRenderFunc((PassData data, RenderGraphContext ctx) =>
                {
                    var cmd = ctx.cmd;
                    int w = renderingData.cameraData.camera.scaledPixelWidth;
                    int h = renderingData.cameraData.camera.scaledPixelHeight;

                    cmd.GetTemporaryRT(TempTargetId, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);

                    material.SetInt(PatternIndex, settings.patternIndex.value);
                    material.SetFloat(DitherThreshold, settings.ditherThreshold.value);
                    material.SetFloat(DitherStrength, settings.ditherStrength.value);
                    material.SetFloat(DitherScale, settings.ditherScale.value);

                    cmd.Blit(colorTarget, TempTargetId);
                    cmd.Blit(TempTargetId, colorTarget, material);
                    cmd.ReleaseTemporaryRT(TempTargetId);
                });
        }

        private class PassData { } // URP17 necesita un tipo de passData vacío
    }
}
