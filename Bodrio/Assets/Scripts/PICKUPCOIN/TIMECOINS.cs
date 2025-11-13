using UnityEngine;

public class TIMECOINS: MonoBehaviour
{
    public float timeBonus = 10f; // tiempo que añade al recoger la moneda
    public CountDownScript countdownScript; // referencia a tu script de contador

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // solo si el jugador recoge la moneda
        {
            if (countdownScript != null)
            {
                countdownScript.currentTime += timeBonus; // sumar tiempo
            }

            Destroy(gameObject); // destruye la moneda al recogerla
        }
    }
}