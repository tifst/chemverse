using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance;

    [Header("Prompt UI")]
    public GameObject promptPanel;         // panel utama (misal: PromptPanel)
    public TextMeshProUGUI promptText;     // teks di dalam panel

    void Awake()
    {
        Instance = this;

        if (promptPanel != null)
            promptPanel.SetActive(false);
    }

    // Tampilkan prompt
    public void Show(string text)
    {
        if (promptText != null)
            promptText.text = text;

        if (promptPanel != null)
            promptPanel.SetActive(true);
    }

    // Sembunyikan prompt
    public void Hide()
    {
        if (promptPanel != null)
            promptPanel.SetActive(false);
    }
}