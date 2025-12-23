using UnityEngine;
using TMPro;
using System.Collections;

public class OrderNotificationUI : MonoBehaviour
{
    public static OrderNotificationUI Instance;

    [Header("UI Components")]
    public CanvasGroup canvasGroup;
    public TMP_Text messageText;

    [Header("Settings")]
    public float showDuration = 2f;   // durasi tampil
    public float fadeSpeed = 2f;      // kecepatan fade in/out

    void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0;
    }

    public void ShowMessage(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(ShowRoutine(msg));
    }

    private IEnumerator ShowRoutine(string msg)
    {
        messageText.text = msg;

        // Fade in
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }

        yield return new WaitForSeconds(showDuration);

        // Fade out
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}