using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class DitheringRenderFeature : ScriptableRendererFeature
    {
        DitheringPass ditheringPass;

        public override void Create()
        {
            ditheringPass = new DitheringPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (ditheringPass == null)
                return;

            renderer.EnqueuePass(ditheringPass);
        }
    }

    public class DitheringPass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "PostEffect/Dithering";
        static readonly string k_RenderTag = "Render Dithering Effects";

        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetDithering");

        static readonly int PatternIndex = Shader.PropertyToID("_PatternIndex");
        static readonly int DitherThreshold = Shader.PropertyToID("_DitherThreshold");
        static readonly int DitherStrength = Shader.PropertyToID("_DitherStrength");
        static readonly int DitherScale = Shader.PropertyToID("_DitherScale");

        Dithering dithering;
        Material ditheringMaterial;
        RTHandle cameraColorTarget;

        public DitheringPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found: " + shaderPath);
                return;
            }
            ditheringMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (ditheringMaterial == null)
                return;

            if (!renderingData.cameraData.postProcessEnabled)
                return;

            var stack = VolumeManager.instance.stack;
            dithering = stack.GetComponent<Dithering>();
            if (dithering == null || !dithering.IsActive())
                return;

            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            var source = cameraColorTarget;
            int destination = TempTargetId;

            int w = cameraData.camera.scaledPixelWidth;
            int h = cameraData.camera.scaledPixelHeight;

            ditheringMaterial.SetInt(PatternIndex, dithering.patternIndex.value);
            ditheringMaterial.SetFloat(DitherThreshold, dithering.ditherThreshold.value);
            ditheringMaterial.SetFloat(DitherStrength, dithering.ditherStrength.value);
            ditheringMaterial.SetFloat(DitherScale, dithering.ditherScale.value);

            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, ditheringMaterial, 0);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(TempTargetId);
        }
    }
}
