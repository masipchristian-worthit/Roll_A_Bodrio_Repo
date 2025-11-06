using UnityEngine;

public class DoorTeethTrigger : MonoBehaviour
{
    public Animator doorAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerPickupState.TeethPickup)
            {
                doorAnimator.SetBool("TeethOpen", true);
            }
            else
            {
            }
        }
    }

    // Opcional: cerrar la puerta al salir
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && PlayerPickupState.TeethPickup)
        {
            doorAnimator.SetBool("TeethOpen", false);
        }
    }
}