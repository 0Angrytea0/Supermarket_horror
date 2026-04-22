using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage;

    [Range(0f, 1f)] public float currentAlpha = 0f;
    private float targetAlpha = 0f;
    public float fadeSpeed = 3f;

    void Update()
    {
        if (fadeImage == null) return;

        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.unscaledDeltaTime);

        Color c = fadeImage.color;
        c.a = currentAlpha;
        fadeImage.color = c;
    }

    public void SetTargetAlpha(float alpha)
    {
        targetAlpha = Mathf.Clamp01(alpha);
    }

    public void SetAlphaInstant(float alpha)
    {
        currentAlpha = Mathf.Clamp01(alpha);
        targetAlpha = currentAlpha;

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = currentAlpha;
            fadeImage.color = c;
        }
    }
}