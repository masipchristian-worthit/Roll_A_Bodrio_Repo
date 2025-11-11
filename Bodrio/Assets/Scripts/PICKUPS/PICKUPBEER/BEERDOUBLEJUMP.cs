using UnityEngine;

public class BeerDoubleJump : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.canDoubleJump = true; //activa el doble salto
                Debug.Log("¡Doble salto activado!");
            }

            gameObject.SetActive(false);
        }
    }
}