using UnityEngine;

public class ColliderMaxVel : MonoBehaviour
{
    // Referencias necesarias
    private PlayerController playerController;
    private PlayerDash playerDash;

    // Estados de restricción
    private bool isInsideTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Obtenemos las referencias al PlayerController y PlayerDash
            playerController = other.GetComponent<PlayerController>();
            playerDash = other.GetComponent<PlayerDash>();

            if (playerController != null)
            {
                isInsideTrigger = true;
            }

            // Desactivar dash
            if (playerDash != null)
            {
                playerDash.enabled = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false;

            // Restaurar dash
            if (playerDash != null)
            {
                playerDash.enabled = true;
            }
        }
    }

    private void Update()
    {
        if (isInsideTrigger && playerController != null)
        {
            // Bloqueamos movimiento en eje Z (adelante/atrás)
            Vector2 currentInput = playerController.moveInput;

            // Solo mantenemos movimiento lateral (X)
            playerController.moveInput = new Vector2(currentInput.x, 0f);
        }
    }
}
