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
            fogPass = new FogPass(RenderPassEvent.AfterRendering);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (fogPass == null) return;

            // cameraColorTargetHandle está marcado obsoleto en algunas versiones de URP,
            // lo usamos por compatibilidad (Render Graph es la alternativa moderna).
#pragma warning disable CS0619
            fogPass.Setup(renderer.cameraColorTargetHandle);
#pragma warning restore CS0619

            renderer.EnqueuePass(fogPass);
        }
    }

    public class FogPass : ScriptableRenderPass
    {
        private const string ShaderPath = "PostEffect/Fog";
        private const string ProfilerTag = "Render Fog Effects";

        private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        private static readonly int TempTargetId = Shader.PropertyToID("_TempTargetFog");
        private static readonly int FogDensity = Shader.PropertyToID("_FogDensity");
        private static readonly int FogDistance = Shader.PropertyToID("_FogDistance");
        private static readonly int FogColor = Shader.PropertyToID("_FogColor");
        private static readonly int FogNear = Shader.PropertyToID("_FogNear");
        private static readonly int FogFar = Shader.PropertyToID("_FogFar");
        private static readonly int FogAltScale = Shader.PropertyToID("_FogAltScale");
        private static readonly int FogThinning = Shader.PropertyToID("_FogThinning");
        private static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
        private static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");

        private Fog fogSettings;
        private Material fogMaterial;
        private RenderTargetIdentifier currentTarget;

        public FogPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find(ShaderPath);
            if (shader == null)
            {
                Debug.LogError($"Shader not found at path: {ShaderPath}");
                return;
            }
            fogMaterial = CoreUtils.CreateEngineMaterial(shader);
        }

        // Setup usando RenderTargetIdentifier (compatible con la mayoría de versiones URP)
        public void Setup(in RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }

        // El compilador solicitó marcar esta sobrecarga como Obsolete (para evitar CS0672).
        [System.Obsolete("This override matches an obsolete base Execute signature required by this URP version.")]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (fogMaterial == null)
            {
                Debug.LogWarning("Fog material not created.");
                return;
            }

            if (!renderingData.cameraData.postProcessEnabled) return;

            var stack = VolumeManager.instance.stack;
            fogSettings = stack.GetComponent<Fog>();
            if (fogSettings == null || !fogSettings.IsActive()) return;

            var cmd = CommandBufferPool.Get(ProfilerTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var source = currentTarget;
            int destination = TempTargetId;

            int w = cameraData.camera.scaledPixelWidth;
            int h = cameraData.camera.scaledPixelHeight;

            cameraData.camera.depthTextureMode |= DepthTextureMode.Depth;

            // Setear parámetros del shader desde el Volume
            fogMaterial.SetFloat(FogDensity, fogSettings.fogDensity.value);
            fogMaterial.SetFloat(FogDistance, fogSettings.fogDistance.value);
            fogMaterial.SetColor(FogColor, fogSettings.fogColor.value);
            fogMaterial.SetFloat(FogNear, fogSettings.fogNear.value);
            fogMaterial.SetFloat(FogFar, fogSettings.fogFar.value);
            fogMaterial.SetFloat(FogAltScale, fogSettings.fogAltScale.value);
            fogMaterial.SetFloat(FogThinning, fogSettings.fogThinning.value);
            fogMaterial.SetFloat(NoiseScale, fogSettings.noiseScale.value);
            fogMaterial.SetFloat(NoiseStrength, fogSettings.noiseStrength.value);

            // Preparar RT temporal
            cmd.GetTemporaryRT(destination, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);

            // Blit: original -> temporal
            cmd.Blit(source, destination);

            // Aplicar efecto y devolver temporal -> original
            cmd.Blit(destination, source, fogMaterial, 0);

            // Liberar temporal
            cmd.ReleaseTemporaryRT(destination);
        }
    }
}
