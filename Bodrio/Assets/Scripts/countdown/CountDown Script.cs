using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CountDownScript : MonoBehaviour
{
    private static CountDownScript instance;

    [Header("Timer Settings")]
    public float startTime = 120f;
    [HideInInspector] public float currentTime;
    [HideInInspector] public bool isPaused = false; // <-- agregado

    [Header("UI References")]
    public TextMeshProUGUI countdownText;
    public Image blackFadeImage;

    [Header("Fade Settings")]
    public float fadeDuration = 5f;
    [HideInInspector] public bool isFading = false;
    public float fadeTimer = 0f;
    public float blackScreenDuration = 2f;
    private bool blackScreenActive = false;
    private float blackScreenTimer = 0f;

    private void Awake()
    {
        // mantener instancia única
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // iniciar o mantener tiempo
        if (currentTime <= 0)
            currentTime = startTime;

        if (blackFadeImage != null)
        {
            Color c = blackFadeImage.color;
            c.a = 0f;
            blackFadeImage.color = c;
        }
    }

    private void Update()
    {
        // si pausa, no bajar tiempo
        if (isPaused) return;

        // pantalla negra activa
        if (blackScreenActive)
        {
            blackScreenTimer += Time.deltaTime;


            if (blackScreenTimer >= blackScreenDuration)
            {
                blackScreenActive = false;
                SceneManager.LoadScene("SCN_Menu");
            }

            return;
        }


        // efecto fade
        if (isFading)
        {
            DoFade();
            return;
        }

        // restar tiempo
        currentTime -= Time.deltaTime;
        if (currentTime < 0) currentTime = 0;

        // mostrar texto
        if (countdownText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // tiempo acabado
        if (currentTime <= 0 && !isFading)
        {
            CountdownFinished();
        }
    }

    private void CountdownFinished()
    {
        isFading = true;
        fadeTimer = 0f;
    }

    private void DoFade()
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // volver a vincular UI si se destruyó en el cambio de escena
        if (countdownText == null)
            countdownText = FindFirstObjectByType<TextMeshProUGUI>();

        if (blackFadeImage == null)
        {
            GameObject fadeObj = GameObject.FindGameObjectWithTag("BlackFade");

            if (fadeObj != null)
                blackFadeImage = fadeObj.GetComponent<Image>();
            else
                Debug.LogWarning("No existe BlackFsade Image");
        }
    }
}
