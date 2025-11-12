using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (fadeCanvasGroup != null)
            StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeIn()
    {
        fadeCanvasGroup.alpha = 1;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = 1 - (timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0;
    }

    IEnumerator FadeOut(string sceneName)
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = timer / fadeDuration;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
