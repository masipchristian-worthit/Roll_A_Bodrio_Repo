using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform spawnPoint; 
    private Rigidbody playerRb;
    private PlayerController playerController;

    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();

    }

    public void Respawn()
    {
     
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        if (playerController != null)
        {
            playerController.isGrounded = true;
            playerController.hasJumpedOnce = false;
            playerController.isClimbing = false;
            playerController.playerRb.useGravity = true;
        }
    }
}
