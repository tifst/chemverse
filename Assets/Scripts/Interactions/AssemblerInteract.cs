using UnityEngine;

public class AssemblerInteract : MonoBehaviour, IInteractable
{
    [Header("Semua Slot (Gas, Fluid, Solid)")]
    public SlotInteract[] allSlots;                   // drag semua slot ke sini
    public OrderManager orderManager;

    [Header("UI Panel")]
    public GameObject assemblePanel;                  // panel merge
    public AssemblerController assemblerController;   // update UI

    public static bool UIActive = false;

    // INTERACT
    public void OnInteractRight() => ShowPanel();
    public void OnInteractLeft()  => ShowPanel();

    public string GetPromptMessage()
    {
        return "[E] Buka Panel Assembler";
    }

    // TAMPILKAN PANEL MERGE
    void ShowPanel()
    {
        if (UIActive) return;

        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.Hide();

        assemblePanel.SetActive(true);
        UIActive = true;

        if (assemblerController != null)
            assemblerController.OpenPanel();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // DIPANGGIL DARI TOMBOL [MERGE] DI UI
    public void OnMergeButtonPressed()
    {
        ProcessReaction();

        assemblePanel.SetActive(false);
        UIActive = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // PROSES PENGGABUNGAN 6 SLOT (Gas, Fluid, Solid)
    void ProcessReaction()
    {
        if (assemblerController == null)
        {
            Debug.LogWarning("❌ AssemblerController belum di-assign di Inspector!");
            return;
        }

        int qtyH = assemblerController.GetQuantity("H");
        int qtyO = assemblerController.GetQuantity("O");
        int qtyNa = assemblerController.GetQuantity("Na");
        int qtyCl = assemblerController.GetQuantity("Cl");
        int qtyH2O = assemblerController.GetQuantity("H2O");

        string result = "";
        string category = "";

        if (qtyH == 2 && qtyO == 1)
        {
            result = "H2O";
            category = "Fluid";
        }
        else if (qtyH == 2 && qtyO == 0)
        {
            result = "H2";
            category = "Gas";
        }
        else if (qtyO == 2 && qtyH == 0)
        {
            result = "O2";
            category = "Gas";
        }
        else if (qtyNa == 1 && qtyCl == 1)
        {
            result = "NaCl";
            category = "Solid";
        }
        else if (qtyNa == 1 && qtyH2O == 1)
        {
            result = "NaOH";
            category = "Fluid";
        }
        else
        {
            InteractionPromptUI.Instance.Show("❌ Kombinasi bahan tidak valid!");
            Debug.Log("❌ Kombinasi tidak valid!");
            return;
        }

        Debug.Log($"✅ Produk berhasil dibuat: {result} [{category}]");
        InteractionPromptUI.Instance.Show($"✅ Produk berhasil: {result}");

        if (orderManager == null || string.IsNullOrEmpty(orderManager.GetCurrentOrder()))
        {
            InteractionPromptUI.Instance.Show("⚠️ Tidak ada order aktif!");
        }
        else if (result == orderManager.GetCurrentOrder())
        {
            orderManager.CompleteOrder();
            InteractionPromptUI.Instance.Show($"🎯 Order {result} selesai!");
        }
        else
        {
            InteractionPromptUI.Instance.Show($"❌ Produk salah ({result})");
        }

        foreach (var slot in allSlots)
            slot.Clear();
    }

    // CLOSE PANEL
    public void CloseAssembler()
    {
        assemblePanel.SetActive(false);
        UIActive = false;

        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.Show("");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}