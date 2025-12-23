using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FormulaController : MonoBehaviour
{
    [Header("UI Elemen")]
    public TMP_Text formulaNameText;
    public TMP_Text formulaSymbolText;
    public TMP_Text formulaDescriptionText;
    public Image formulaImage; // opsional (bisa dikosongkan)

    // DIPANGGIL OLEH FormulaInteract
    public void ShowFormula(string name, string symbol, string description)
    {
        if (formulaNameText != null) 
            formulaNameText.text = name;

        if (formulaSymbolText != null)
            formulaSymbolText.text = symbol;

        if (formulaDescriptionText != null)
            formulaDescriptionText.text = description;
    }

    public void ClearFormula()
    {
        if (formulaNameText != null) formulaNameText.text = "";
        if (formulaSymbolText != null) formulaSymbolText.text = "";
        if (formulaDescriptionText != null) formulaDescriptionText.text = "";
    }
}