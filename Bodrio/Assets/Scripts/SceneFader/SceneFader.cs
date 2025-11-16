using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    
    public CanvasGroup fadeCanvasGroup;

    [Header("Duration")]
    public float fadeOutDuration = 5f;
    public float blackScreenDuration = 3f;

    void Awake()
    {
        if (fadeCanvasGroup != null)
            StartCoroutine(FadeInFromBlack());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeInFromBlack()
    {
        fadeCanvasGroup.alpha = 1;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = 1 - t;
            yield return null;
        }

        fadeCanvasGroup.alpha = 0;
    }

    IEnumerator FadeOut(string sceneName)
    {
        float t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = t / fadeOutDuration;
            yield return null;
        }

        fadeCanvasGroup.alpha = 1;

        yield return new WaitForSecondsRealtime(blackScreenDuration);
        SceneManager.LoadScene(sceneName);
    }
}
