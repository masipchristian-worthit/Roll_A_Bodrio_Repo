using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    [SerializeField] private string targetScene;       // Nombre de la escena a cargar
    [SerializeField] private string targetSpawnID;     // ID del spawn en la escena destino

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneTransitionManager.LastDoorID = targetSpawnID;

            SceneManager.LoadScene(targetScene);
        }
    }
}