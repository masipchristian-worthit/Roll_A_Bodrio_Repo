using UnityEngine;
 // <-- ⚠️ CORRECCIÓN: Añade esta línea

public class ColliderMaxVel : MonoBehaviour
{
    // Referencias necesarias
    private PlayerController playerController;
    private Dash playerDash;

    // Estados de restricción
    private bool isInsideTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Obtenemos las referencias al PlayerController y PlayerDash
            // (Esto sobrescribirá al jugador anterior si entra uno nuevo)
            playerController = other.GetComponent<PlayerController>();
            playerDash = other.GetComponent<Dash>();

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
            // Comprobamos si el jugador que SALE es el que teníamos guardado
            PlayerController exitingController = other.GetComponent<PlayerController>();

            if (exitingController != null && exitingController == playerController)
            {
                isInsideTrigger = false;

                // Restaurar dash
                if (playerDash != null)
                {
                    playerDash.enabled = true;
                }

                // Limpiamos las referencias
                playerController = null;
                playerDash = null;
            }
        }
    }

    private void Update()
    {
        // Solo aplica la restricción si el jugador está dentro
        // y la referencia es válida
        if (isInsideTrigger && playerController != null)
        {
            // Bloqueamos movimiento en eje Z (adelante/atrás)
            Vector2 currentInput = playerController.moveInput;

            // Solo mantenemos movimiento lateral (X)
            playerController.moveInput = new Vector2(currentInput.x, 0f);
        }
    }
}