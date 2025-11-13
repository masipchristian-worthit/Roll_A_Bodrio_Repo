using UnityEngine;
using System.Collections;

public class TEETHPICKUP : MonoBehaviour
{
    public string parentName = "AnimationWallsParent";
    public bool useTrigger = true;

    [Header("Texto del mensaje en pantalla")]
    public GameObject textoUI; // Arrastra el texto del Canvas
    public float duracionMensaje = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (useTrigger && other.CompareTag("Player"))
            ActivatePickup();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!useTrigger && collision.gameObject.CompareTag("Player"))
            ActivatePickup();
    }

    private void ActivatePickup()
    {
        PlayerPickupState.TeethPickup = true;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("WallTeeth"))
            Destroy(obj);

        GameObject parent = GameObject.Find(parentName);
        if (parent != null)
        {
            foreach (Transform child in parent.transform)
            {
                if (child.CompareTag("AnimationWallTeeth"))
                {
                    child.gameObject.SetActive(true);
                }
            }
        }

        if (textoUI != null)
        {
            textoUI.SetActive(true);
            StartCoroutine(DesactivarDespuesDeTiempo(textoUI, duracionMensaje));
        }

        Destroy(gameObject);
    }

    private IEnumerator DesactivarDespuesDeTiempo(GameObject objeto, float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        objeto.SetActive(false);
    }
}