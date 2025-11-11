using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountDownScript : MonoBehaviour
{
    [Header("Configuración del Tiempo")]
    public float startTime = 120f;
    private float currentTime;

    [Header("Referencias de UI")]
    public TextMeshProUGUI countdownText;
    public Image blackFadeImage;

    [Header("Configuración del Fade")]
    public float fadeDuration = 5f;
    [HideInInspector] public bool isFading = false;
    public float fadeTimer = 0f;

    [Header("Pantalla Negra")]
    public float blackScreenDuration = 10f;
    private bool blackScreenActive = false;
    private float blackScreenTimer = 0f;

    [Header("Control de Pausa")]
    [HideInInspector] public bool isPaused = false; // <-- NUEVO: pausa del contador

    void Start()
    {
        currentTime = startTime;

        if (blackFadeImage != null)
        {
            Color c = blackFadeImage.color;
            c.a = 0f;
            blackFadeImage.color = c;
        }
    }

    void Update()
    {
        // Si la pantalla negra está activa
        if (blackScreenActive)
        {
            blackScreenTimer += Time.deltaTime;

            if (blackScreenTimer >= blackScreenDuration)
            {
                Debug.Log("Tiempo de pantalla negra terminado. (Aquí puedes cargar la escena de partida perdida)");
                blackScreenActive = false;
                // Aquí podrías hacer: SceneManager.LoadScene("NombreDeTuEscena");
            }
            return;
        }

        // Si está en proceso de fade
        if (isFading)
        {
            DoFade();
            return;
        }

        // Si no está pausado, sigue descontando tiempo
        if (!isPaused)
        {
            currentTime -= Time.deltaTime;

            if (currentTime < 0)
                currentTime = 0;
        }

        // Mostrar el tiempo en pantalla
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Cuando el tiempo llega a 0
        if (currentTime <= 0 && !isFading)
        {
            CountdownFinished();
        }
    }

    void CountdownFinished()
    {
        Debug.Log("¡You're DEAD!");
        isFading = true;
        fadeTimer = 0f;
    }

    void DoFade()
    {
        if (blackFadeImage == null) return;

        fadeTimer += Time.deltaTime;
        float progress = fadeTimer / fadeDuration;

        Color c = blackFadeImage.color;
        c.a = Mathf.Lerp(0f, 1f, progress);
        blackFadeImage.color = c;

        if (progress >= 1f)
        {
            isFading = false;
            Debug.Log("Fade Completed");

            blackScreenActive = true;
            blackScreenTimer = 0f;
        }
    }
}