using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PSX
{
    public class FogRenderFeature : ScriptableRendererFeature
    {
        FogPass fogPass;

        public override void Create()
        {
            fogPass = new FogPass(RenderPassEvent.BeforeRenderingPostProcessing);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (fogPass == null)
                return;

            renderer.EnqueuePass(fogPass);
        }
    }

    public class FogPass : ScriptableRenderPass
    {
        private static readonly string shaderPath = "PostEffect/Fog";
        static readonly string k_RenderTag = "Render Fog Effects";

        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetFog");

        static readonly int FogColor = Shader.PropertyToID("_FogColor");
        static readonly int FogDensity = Shader.PropertyToID("_FogDensity");
        static readonly int FogNear = Shader.PropertyToID("_FogNear");
        static readonly int FogFar = Shader.PropertyToID("_FogFar");
        static readonly int FogAltScale = Shader.PropertyToID("_FogAltScale");
        static readonly int FogThinning = Shader.PropertyToID("_FogThinning");
        static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
        static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");

        Fog fog;
        Material fogMaterial;
        RTHandle cameraColorTarget;

        public FogPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(shaderPath);
            if (shader == null)
            {
                Debug.LogError("Shader not found: " + shaderPath);
                return;
            }
            fogMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (fogMaterial == null)
                return;

            if (!renderingData.cameraData.postProcessEnabled)
                return;

            var stack = VolumeManager.instance.stack;
            fog = stack.GetComponent<Fog>();
            if (fog == null || !fog.IsActive())
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

            fogMaterial.SetColor(FogColor, fog.fogColor.value);
            fogMaterial.SetFloat(FogDensity, fog.fogDensity.value);
            fogMaterial.SetFloat(FogNear, fog.fogNear.value);
            fogMaterial.SetFloat(FogFar, fog.fogFar.value);
            fogMaterial.SetFloat(FogAltScale, fog.fogAltScale.value);
            fogMaterial.SetFloat(FogThinning, fog.fogThinning.value);
            fogMaterial.SetFloat(NoiseScale, fog.noiseScale.value);
            fogMaterial.SetFloat(NoiseStrength, fog.noiseStrength.value);

            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, fogMaterial, 0);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(TempTargetId);
        }
    }
}

