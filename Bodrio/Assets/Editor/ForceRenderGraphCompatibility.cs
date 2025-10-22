using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[InitializeOnLoad]
public static class ForceRenderGraphCompatibility
{
    static ForceRenderGraphCompatibility()
    {
        // Esperamos a que el editor cargue para evitar nulls al inicio
        EditorApplication.delayCall += () =>
        {
            var pipeline = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipeline == null)
            {
                Debug.LogWarning("⚠️ No URP Asset found in Graphics Settings.");
                return;
            }

            var serializedObject = new SerializedObject(pipeline);

            // Buscar propiedad correcta según versión de URP
            SerializedProperty renderGraphProp =
                serializedObject.FindProperty("m_RenderGraphEnabled") ??
                serializedObject.FindProperty("m_RenderGraph");

            if (renderGraphProp != null)
            {
                renderGraphProp.boolValue = false;
                serializedObject.ApplyModifiedProperties();
                Debug.Log("✅ RenderGraph disabled successfully — Compatibility Mode enabled.");
            }
            else
            {
                Debug.LogWarning("⚠️ RenderGraph property not found in this URP version. Compatibility mode may already be default.");
            }
        };
    }
}


