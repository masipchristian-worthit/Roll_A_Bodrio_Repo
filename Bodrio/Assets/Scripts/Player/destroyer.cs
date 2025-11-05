using UnityEngine;

public class PlayerWallDestroy : MonoBehaviour
{
    public int destroyCharges = 0; // cargas disponibles
    public bool consumeOnHit = true; // si se consume una carga al destruir una pared

    // Llamado por el Pickup
    public void AddDestroyCharges(int amount)
    {
        destroyCharges += amount;
        // aquí podrías actualizar UI
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("desWall") && destroyCharges > 0)
        {
            Destroy(collision.gameObject); // destruye la pared
            if (consumeOnHit) destroyCharges--;
            // opcional: reproducir sonido/partículas aquí
        }
    }
}