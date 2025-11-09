using UnityEngine;

public class FadeInTrigger : MonoBehaviour
{
    public CountDownScript countdownScript; // referencia al script del fade
    public OutOfTimePlayer outOfTimePlayer; // referencia al script que congela al player

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (countdownScript != null)
            {
                // Inicia el fade in
                countdownScript.isFading = true;
                countdownScript.fadeTimer = 0f;
                Debug.Log("Fade in activado por el trigger.");
            }

            if (outOfTimePlayer != null)
            {
                // Forzamos que el freeze se ejecute (igual que si se acabara el tiempo)
                outOfTimePlayer.enabled = true;
            }
        }
    }
}