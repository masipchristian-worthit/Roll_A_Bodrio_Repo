using UnityEngine;
using UnityEngine.Video;

public class ReproducirVideoTrigger : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string tagJugador = "Player";
    public CountDownScript countDownScript; // Arrastra tu objeto con el temporizador

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            if (videoPlayer != null)
                videoPlayer.Play();

            if (countDownScript != null)
                countDownScript.isPaused = true; // Pausa el contador
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            if (videoPlayer != null)
                videoPlayer.Pause();

            if (countDownScript != null)
                countDownScript.isPaused = false; // Reanuda el contador
        }
    }
}