using UnityEngine;


public class MusicPauseZone : MonoBehaviour
{
    [Header("Referencia a la Música")]
    [Tooltip("Arrastra aquí el objeto que tiene el AudioSource de la música de fondo")]
    public AudioSource musicAudioSource;

    private void Start()
    {

    }

    /// <summary>
    /// Se llama cuando otro collider entra en este trigger.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si el objeto que ha entrado es el jugador (usando su Tag)
        if (other.CompareTag("Player"))
        {
            // Si tenemos la referencia y la música está sonando, la pausamos.
            if (musicAudioSource != null && musicAudioSource.isPlaying)
            {
                musicAudioSource.Pause();
                Debug.Log("Música pausada.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Comprobamos si el objeto que ha salido es el jugador
        if (other.CompareTag("Player"))
        {
            // Si tenemos la referencia, reanudamos la música.
            // Usamos UnPause() para que continúe exactamente donde se quedó.
            if (musicAudioSource != null)
            {
                musicAudioSource.UnPause();
                Debug.Log("Música reanudada.");
            }
        }
    }
}
