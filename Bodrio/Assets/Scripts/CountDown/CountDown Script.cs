using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CountDownScript : MonoBehaviour
{
    public float startTime = 120f;
    private float currentTime;

    public TextMeshProUGUI countdownText;
    public Image blackFadeImage;
    public float fadeDuration = 5f;

    private bool isFading = false;
    private float fadeTimer = 0f;

    void Start()
    {
        currentTime = startTime;

        if (blackFadeImage != null )
        {

            blackFadeImage.fillAmount = 0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            DoFade();

            return;
            }

            currentTime -= Time.deltaTime;

        if (currentTime < 0)

        {
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
        Debug.Log("¡You're DEAD!");
        isFading = true;
        fadeTimer = 0f;

    }
void DoFade()
    {

        if (blackFadeImage == null) return;

        fadeTimer += Time.deltaTime;
        float progress = fadeTimer / fadeDuration;

        blackFadeImage.fillAmount = Mathf.Lerp(0f, 1f, progress);

        if (progress >= 1f)
        {
            isFading = false;
            Debug.Log("Fade Completed");

        }
    }

    }
