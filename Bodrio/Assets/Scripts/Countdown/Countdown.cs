using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountDownScript : MonoBehaviour
{
    public float startTime = 120f;
    private float currentTime;
    public TextMeshProUGUI countdownText;
    public Image blackFadeImage;
    public float fadeDuration = 5f;
    [HideInInspector] public bool isFading = false;
    public float fadeTimer = 0f;
    public float blackScreenDuration = 10f;
    private bool blackScreenActive = false;
    private float blackScreenTimer = 0f;

    [HideInInspector] public bool isPaused = false; // <-- NUEVO: variable para pausar

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
        if (blackScreenActive)
        {
            blackScreenTimer += Time.deltaTime;
            if (blackScreenTimer >= blackScreenDuration)
            {
                blackScreenActive = false;
            }
            return;
        }

        if (isFading)
        {
            DoFade();
            return;
        }

        if (!isPaused) // <-- solo resta tiempo si no está pausado
        {
            currentTime -= Time.deltaTime;
            if (currentTime < 0)
                currentTime = 0;
        }

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 0 && !isFading)
        {
            CountdownFinished();
        }
    }

    void CountdownFinished()
    {
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
            blackScreenActive = true;
            blackScreenTimer = 0f;
        }
    }
}