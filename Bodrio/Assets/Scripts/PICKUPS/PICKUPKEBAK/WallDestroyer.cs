using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
    public int destroyCharges = 1;
    public bool destroyOnPickup = true;

    [Header("Texto de aviso en pantalla")]
    public GameObject textoUI; // arrastra el objeto del Canvas
    public float duracionMensaje = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerWallDestroy p = other.GetComponent<PlayerWallDestroy>();
            if (p != null)
            {
                p.AddDestroyCharges(destroyCharges);

                if (textoUI != null)
                {
                    textoUI.SetActive(true);
                    textoUI.GetComponent<MonoBehaviour>().StartCoroutine(DesactivarDespuesDeTiempo(textoUI, duracionMensaje));
                }

                if (destroyOnPickup)
                    Destroy(gameObject);
            }
        }
    }

    private IEnumerator DesactivarDespuesDeTiempo(GameObject objeto, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        objeto.SetActive(false);
    }
}