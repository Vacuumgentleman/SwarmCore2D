using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ScreenFadeToMenu : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    bool isFading;

    void Awake()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    public void FadeToMenu()
    {
        if (isFading || fadeImage == null)
            return;

        gameObject.SetActive(true); 
        StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        isFading = true;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float a = Mathf.Clamp01(t / fadeDuration);

            Color c = fadeImage.color;
            c.a = a;
            fadeImage.color = c;

            yield return null;
        }

        yield return new WaitForSeconds(0.2f); 

        int current = SceneManager.GetActiveScene().buildIndex;
        int previous = Mathf.Max(0, current - 1);

        SceneManager.LoadScene(previous);
    }
}