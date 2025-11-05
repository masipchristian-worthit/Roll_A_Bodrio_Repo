using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int destroyCharges = 1;
    public bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWallDestroy p = other.GetComponent<PlayerWallDestroy>();
            if (p != null)
            {
                p.AddDestroyCharges(destroyCharges);
                if (destroyOnPickup) Destroy(gameObject);
            }
        }
    }
}