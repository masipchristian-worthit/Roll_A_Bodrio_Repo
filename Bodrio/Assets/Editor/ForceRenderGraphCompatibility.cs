using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        private FogPass fogPass;

        public override void Create()
        {
            fogPass = new FogPass(RenderPassEvent.AfterRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(fogPass);
        }
    }

    public class FogPass : ScriptableRenderPass
    {
        private const string ShaderPath = "PostEffect/Fog";
        private const string ProfilerTag = "Render Fog Effects";

        private static readonly int TempTargetId = Shader.PropertyToID("_TempTargetFog");
        private Material fogMaterial;
        private Fog fogSettings;

        public FogPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(ShaderPath);
            if (shader == null) Debug.LogError($"Shader not found: {ShaderPath}");
            fogMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.postProcessEnabled || fogMaterial == null) return;

            var stack = VolumeManager.instance.stack;
            fogSettings = stack.GetComponent<Fog>();
            if (fogSettings == null || !fogSettings.IsActive()) return;

            var cmd = CommandBufferPool.Get(ProfilerTag);

#pragma warning disable CS0618
            var source = renderingData.cameraData.renderer.cameraColorTargetHandle;
#pragma warning restore CS0618

            int w = renderingData.cameraData.camera.scaledPixelWidth;
            int h = renderingData.cameraData.camera.scaledPixelHeight;

            fogMaterial.SetFloat("_FogDensity", fogSettings.fogDensity.value);
            fogMaterial.SetFloat("_FogDistance", fogSettings.fogDistance.value);
            fogMaterial.SetColor("_FogColor", fogSettings.fogColor.value);
            fogMaterial.SetFloat("_FogNear", fogSettings.fogNear.value);
            fogMaterial.SetFloat("_FogFar", fogSettings.fogFar.value);
            fogMaterial.SetFloat("_FogAltScale", fogSettings.fogAltScale.value);
            fogMaterial.SetFloat("_FogThinning", fogSettings.fogThinning.value);
            fogMaterial.SetFloat("_NoiseScale", fogSettings.noiseScale.value);
            fogMaterial.SetFloat("_NoiseStrength", fogSettings.noiseStrength.value);

            cmd.GetTemporaryRT(TempTargetId, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(source, TempTargetId);
            cmd.Blit(TempTargetId, source, fogMaterial);
            cmd.ReleaseTemporaryRT(TempTargetId);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
