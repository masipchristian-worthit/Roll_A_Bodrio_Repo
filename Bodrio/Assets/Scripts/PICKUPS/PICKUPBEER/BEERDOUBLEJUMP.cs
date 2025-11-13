using UnityEngine;
using TMPro;
using System.Collections;

public class BeerDoubleJump : MonoBehaviour
{
    [Header("Texto del mensaje en pantalla")]
    public GameObject textoUI; // Arrastra el texto del Canvas
    public float duracionMensaje = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.canDoubleJump = true;
                Debug.Log("¡Doble salto activado!");
            }

            if (textoUI != null)
            {
                textoUI.SetActive(true);
                textoUI.GetComponent<MonoBehaviour>().StartCoroutine(DesactivarDespuesDeTiempo(textoUI, duracionMensaje));
            }

            gameObject.SetActive(false);
        }
    }

    private IEnumerator DesactivarDespuesDeTiempo(GameObject objeto, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        objeto.SetActive(false);
    }
}