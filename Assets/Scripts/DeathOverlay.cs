using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DeathOverlay : MonoBehaviour
{
    public static DeathOverlay I;

    [SerializeField] Image panel;        // the Image on DeathOverlay
    [SerializeField] TMP_Text message;   // the TMP child

    void Awake()
    {
        I = this;
        if (!panel) panel = GetComponent<Image>();
        SetAlpha(0f);
        gameObject.SetActive(true); // keep active; we drive alpha
    }

    void SetAlpha(float a)
    {
        var c = panel.color; c.a = a; panel.color = c;
        if (message) message.alpha = a;
    }

    public IEnumerator FadeInAndHold(float fadeTime, float holdTime, string text)
    {
        if (message && !string.IsNullOrEmpty(text)) message.text = text;

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / fadeTime);
            SetAlpha(k);
            yield return null;
        }
        SetAlpha(1f);
        yield return new WaitForSeconds(holdTime);
    }
}
