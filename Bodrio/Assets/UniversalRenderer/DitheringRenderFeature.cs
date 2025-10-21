using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class DitheringRenderFeature : ScriptableRendererFeature
    {
        private DitheringPass ditheringPass;

        public override void Create()
        {
            ditheringPass = new DitheringPass(RenderPassEvent.AfterRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (ditheringPass == null) return;
            renderer.EnqueuePass(ditheringPass);
        }
    }

    public class DitheringPass : ScriptableRenderPass
    {
        private const string ShaderPath = "PostEffect/Dithering";
        private const string ProfilerTag = "Render Dithering Effects";

        private static readonly int TempTargetId = Shader.PropertyToID("_TempTargetDithering");
        private static readonly int PatternIndex = Shader.PropertyToID("_PatternIndex");
        private static readonly int DitherThreshold = Shader.PropertyToID("_DitherThreshold");
        private static readonly int DitherStrength = Shader.PropertyToID("_DitherStrength");
        private static readonly int DitherScale = Shader.PropertyToID("_DitherScale");

        private Dithering ditheringSettings;
        private Material ditheringMaterial;

        public DitheringPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(ShaderPath);
            if (shader == null)
            {
                Debug.LogError($"Shader not found at path: {ShaderPath}");
                return;
            }
            ditheringMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.postProcessEnabled || ditheringMaterial == null) return;

            var stack = VolumeManager.instance.stack;
            ditheringSettings = stack.GetComponent<Dithering>();
            if (ditheringSettings == null || !ditheringSettings.IsActive()) return;

            var cmd = CommandBufferPool.Get(ProfilerTag);

#pragma warning disable CS0618
            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
#pragma warning restore CS0618

            int w = renderingData.cameraData.camera.scaledPixelWidth;
            int h = renderingData.cameraData.camera.scaledPixelHeight;

            ditheringMaterial.SetInt(PatternIndex, ditheringSettings.patternIndex.value);
            ditheringMaterial.SetFloat(DitherThreshold, ditheringSettings.ditherThreshold.value);
            ditheringMaterial.SetFloat(DitherStrength, ditheringSettings.ditherStrength.value);
            ditheringMaterial.SetFloat(DitherScale, ditheringSettings.ditherScale.value);

            cmd.GetTemporaryRT(TempTargetId, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, TempTargetId);
            cmd.Blit(TempTargetId, source, ditheringMaterial);
            cmd.ReleaseTemporaryRT(TempTargetId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
