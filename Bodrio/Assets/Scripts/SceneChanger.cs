using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneTrigger : MonoBehaviour
{
    [Header("Nombre de la escena a cargar")]
    public string sceneToLoad = "SCN_Maze_Sewers";

    [Header("Etiqueta del jugador")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Entró al trigger. Cargando escena: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}