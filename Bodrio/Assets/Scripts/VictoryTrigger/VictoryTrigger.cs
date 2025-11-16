using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VictoryTrigger : MonoBehaviour
{
    [Header("References")]
    public SceneFader sceneFader;
    public CanvasGroup winMessage;
    public PlayerController player;

    [Header("Timings")]
    public float messageFadeDuration = 2f;
    public float timeBeforeReturn = 5f;

    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(VictorySequence());
        }
    }

    IEnumerator VictorySequence()
    {

        player.enabled = false;
        player.playerRb.linearVelocity = Vector3.zero;
        player.playerRb.angularVelocity = Vector3.zero;



        winMessage.gameObject.SetActive(true);
        winMessage.alpha = 0;


        float t = 0;

        while (t < messageFadeDuration)
        {
            t += Time.deltaTime;
            winMessage.alpha = Mathf.Lerp(0, 1, t / messageFadeDuration);
            yield return null;
        }

        winMessage.alpha = 1;

        // 3. Esperar los 8 segundos totales
        yield return new WaitForSeconds(timeBeforeReturn);

        // 4. Volver al menú principal
        SceneManager.LoadScene("SCN_Menu");


    }
    }

