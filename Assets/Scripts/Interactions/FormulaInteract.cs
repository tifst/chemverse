using UnityEngine;

public class FormulaInteract : MonoBehaviour, IInteractable
{
    [Header("UI Formula Panel")]
    public GameObject formulaPanel;          // Panel UI yang muncul
    public FormulaController formulaController;    // Script yang mengatur teks & data di UI

    [Header("Formula Data")]
    [Tooltip("Nama atau judul senyawa (misal: Air)")]
    public string formulaName = "Oksigen";

    [Tooltip("Rumus senyawa (misal: O₂)")]
    public string formulaSymbol = "O₂";

    [TextArea(2, 4)]
    [Tooltip("Deskripsi senyawa atau penjelasan kimia singkat")]
    public string formulaDescription = "O₂ adalah molekul gas yang terdiri dari dua atom oksigen, sangat penting bagi respirasi makhluk hidup.";

    public static bool UIActive = false;

    // INTERAKSI PLAYER
    public void OnInteractRight() => ShowFormula();
    public void OnInteractLeft()  => ShowFormula();

    public string GetPromptMessage()
    {
        return "[E] Lihat Informasi Senyawa";
    }

    // MENAMPILKAN PANEL FORMULA
    void ShowFormula()
    {
        if (UIActive) return;

         if (InteractionPromptUI.Instance != null)
        InteractionPromptUI.Instance.Hide();

        formulaPanel.SetActive(true);
        UIActive = true;

        if (formulaController != null)
            formulaController.ShowFormula(formulaName, formulaSymbol, formulaDescription);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        InteractionPromptUI.Instance.Hide();
    }

    // DIPANGGIL DARI TOMBOL [TUTUP]
    public void CloseFormula()
    {
        formulaPanel.SetActive(false);
        UIActive = false;

         if (InteractionPromptUI.Instance != null)
        InteractionPromptUI.Instance.Show("");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
