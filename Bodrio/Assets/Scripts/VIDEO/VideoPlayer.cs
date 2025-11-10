using UnityEngine;
using UnityEngine.Video;

public class ReproducirVideoTrigger : MonoBehaviour
{
    public VideoPlayer videoPlayer; // arrastra aquí tu Video Player
    public string tagJugador = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            if (videoPlayer != null)
            {
                videoPlayer.Play();
            }
            else
            {
                Debug.LogError("No asignaste el Video Player en el inspector");
            }
        }
    }
}