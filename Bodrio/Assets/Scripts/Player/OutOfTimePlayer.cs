using UnityEngine;

public class OutOfTimePlayer : MonoBehaviour
{
    [Header("Referecnias")]
    public Rigidbody playerRb;
    public CountDownScript countdownScript;
    public MonoBehaviour playerControllerScript;

    private bool hasFrozen = false;

    void Update()
    {

        if (countdownScript == null || hasFrozen)
            return;

        if (countdownScript.isFading)
        {
            FreezePlayer();

        }
    }

    void FreezePlayer()
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

        hasFrozen = true;
        Debug.Log("Player Frozen");

    }
}
