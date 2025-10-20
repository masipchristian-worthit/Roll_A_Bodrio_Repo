using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class PixelationRenderFeature : ScriptableRendererFeature
    {
        PixelationPass pixelationPass;

        public override void Create()
        {
            pixelationPass = new PixelationPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (pixelationPass == null)
                return;

            // Ya NO usamos cameraColorTargetHandle aquí
            renderer.EnqueuePass(pixelationPass);
        }
    }

    public class PixelationPass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "PostEffect/Pixelation";
        static readonly string k_RenderTag = "Render Pixelation Effects";

        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetPixelation");

        static readonly int WidthPixelation = Shader.PropertyToID("_WidthPixelation");
        static readonly int HeightPixelation = Shader.PropertyToID("_HeightPixelation");
        static readonly int ColorPrecision = Shader.PropertyToID("_ColorPrecision");

        Pixelation pixelation;
        Material pixelationMaterial;
        RTHandle cameraColorTarget;

        public PixelationPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found: " + shaderPath);
                return;
            }
            pixelationMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // Aquí sí podemos acceder al target de cámara correctamente
            cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (pixelationMaterial == null)
                return;

            if (!renderingData.cameraData.postProcessEnabled)
                return;

            var stack = VolumeManager.instance.stack;
            pixelation = stack.GetComponent<Pixelation>();
            if (pixelation == null || !pixelation.IsActive())
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

            pixelationMaterial.SetFloat(WidthPixelation, pixelation.widthPixelation.value);
            pixelationMaterial.SetFloat(HeightPixelation, pixelation.heightPixelation.value);
            pixelationMaterial.SetFloat(ColorPrecision, pixelation.colorPrecision.value);

            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, pixelationMaterial, 0);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
                throw new System.ArgumentNullException("cmd");

            cmd.ReleaseTemporaryRT(TempTargetId);
        }
    }
}


