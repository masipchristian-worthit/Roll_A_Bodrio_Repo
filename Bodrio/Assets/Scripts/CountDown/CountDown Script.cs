using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CountDownScript : MonoBehaviour
{
    public float startTime = 120f;
    private float currentTime;

    public TextMeshProUGUI countdownText;
    public Image blackFadeImage;
    public float fadeDuration = 5f;

    [HideInInspector] public bool isFading = false;
    private float fadeTimer = 0f;

    public float blackScreenDuration = 10f;
    private bool blackScreenActive = false;
    private float blackScreenTimer = 0f;

    void Start()
    {
        currentTime = startTime;

        if (blackFadeImage != null )
        {

            Color c = blackFadeImage.color;
            c.a = 0f;
            blackFadeImage.color = c;
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (blackScreenActive)
        {
            blackScreenTimer += Time.deltaTime;

            if (blackScreenTimer >= blackScreenDuration)
            {
                Debug.Log("Tiempo de pantalla negra terminado. (Aquí puedes cargar la escena de partida perdida)");
                blackScreenActive = false;

            }
            return;
        }

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
