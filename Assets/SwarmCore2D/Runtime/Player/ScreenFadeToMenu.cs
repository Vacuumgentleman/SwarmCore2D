using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ScreenFadeToMenu : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    bool isFading;

    public void FadeToMenu()
    {
        if (isFading)
            return;

        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        isFading = true;

        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float a = Mathf.Clamp01(t / fadeDuration);

            c.a = a;
            fadeImage.color = c;

            yield return null;
        }

        int current = SceneManager.GetActiveScene().buildIndex;
        int previous = Mathf.Max(0, current - 1);

        SceneManager.LoadScene(previous);
    }
}