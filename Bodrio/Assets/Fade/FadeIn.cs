using UnityEngine;

public class TriggerFadeAndFreeze : MonoBehaviour
{
    [Header("Referencias")]
    public CountDownScript countdownScript;       // Script del fade
    public Rigidbody playerRb;                     // Rigidbody del jugador
    public MonoBehaviour playerControllerScript;  // Script de movimiento del jugador

    private bool triggered = false;

    void Awake()
    {
        if (playerRb != null)
            playerRb.isKinematic = false;

        if (playerControllerScript != null)
            playerControllerScript.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (countdownScript != null)
        {
            countdownScript.isFading = true;
            countdownScript.fadeTimer = 0f;

            if (countdownScript.countdownText != null)
                countdownScript.countdownText.gameObject.SetActive(false);

            Debug.Log("Fade in activado por trigger.");
        }

        FreezePlayer();

        Debug.Log("Player frozen por trigger.");
    }

    private void FreezePlayer()
    {
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
            playerRb.isKinematic = true;
        }

        if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false;
        }
    }
}