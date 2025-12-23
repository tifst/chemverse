using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AssemblerController : MonoBehaviour
{
    [Header("Semua Slot (Gas, Fluid, Solid)")]
    public SlotInteract[] allSlots; // drag ke sini semua slot

    [Header("UI Fields")]
    public TMP_Text gasInfoText;
    public TMP_Text gasSlot1Text;
    public TMP_Text gasSlot2Text;
    public TMP_InputField gasSlot1Qty;
    public TMP_InputField gasSlot2Qty;

    public TMP_Text fluidInfoText;
    public TMP_Text fluidSlot1Text;
    public TMP_Text fluidSlot2Text;
    public TMP_InputField fluidSlot1Qty;
    public TMP_InputField fluidSlot2Qty;

    public TMP_Text solidInfoText;
    public TMP_Text solidSlot1Text;
    public TMP_Text solidSlot2Text;
    public TMP_InputField solidSlot1Qty;
    public TMP_InputField solidSlot2Qty;

    public TMP_Text resultText;

    [Header("Reactor")]
    public ReactorTank reactor;

    private List<SlotInteract> gasSlots = new();
    private List<SlotInteract> fluidSlots = new();
    private List<SlotInteract> solidSlots = new();

    // =========================================================
    // DIPANGGIL SAAT PANEL DIBUKA
    // =========================================================
    public void OpenPanel()
    {
        UpdateUISlots();
    }

    // =========================================================
    // UPDATE STATUS SEMUA SLOT DI UI
    // =========================================================
    void UpdateUISlots()
    {
        gasSlots.Clear();
        fluidSlots.Clear();
        solidSlots.Clear();

        foreach (var slot in allSlots)
        {
            if (slot == null || !slot.isFilled) continue;
            switch (slot.slotCategory)
            {
                case "Gas": gasSlots.Add(slot); break;
                case "Fluid": fluidSlots.Add(slot); break;
                case "Solid": solidSlots.Add(slot); break;
            }
        }

        // 🔹 Gas
        gasInfoText.text = $"Slot Gas ({gasSlots.Count})";
        gasSlot1Text.text = gasSlots.Count > 0 ? gasSlots[0].storedItemName : "Kosong";
        gasSlot2Text.text = gasSlots.Count > 1 ? gasSlots[1].storedItemName : "Kosong";
        gasSlot1Qty.text = "0";
        gasSlot2Qty.text = "0";

        // 🔹 Fluid
        fluidInfoText.text = $"Slot Fluid ({fluidSlots.Count})";
        fluidSlot1Text.text = fluidSlots.Count > 0 ? fluidSlots[0].storedItemName : "Kosong";
        fluidSlot2Text.text = fluidSlots.Count > 1 ? fluidSlots[1].storedItemName : "Kosong";
        fluidSlot1Qty.text = "0";
        fluidSlot2Qty.text = "0";

        // 🔹 Solid
        solidInfoText.text = $"Slot Solid ({solidSlots.Count})";
        solidSlot1Text.text = solidSlots.Count > 0 ? solidSlots[0].storedItemName : "Kosong";
        solidSlot2Text.text = solidSlots.Count > 1 ? solidSlots[1].storedItemName : "Kosong";
        solidSlot1Qty.text = "0";
        solidSlot2Qty.text = "0";

        resultText.text = "Belum ada hasil.";
    }

    // =========================================================
    // FUNGSI UNTUK MENGAMBIL QUANTITY DARI ITEM TERTENTU
    // =========================================================
    public int GetQuantity(string itemName)
    {
        int total = 0;

        // 🔹 Gas
        if (gasSlot1Text.text == itemName && int.TryParse(gasSlot1Qty.text, out int g1)) total += g1;
        if (gasSlot2Text.text == itemName && int.TryParse(gasSlot2Qty.text, out int g2)) total += g2;

        // 🔹 Fluid
        if (fluidSlot1Text.text == itemName && int.TryParse(fluidSlot1Qty.text, out int f1)) total += f1;
        if (fluidSlot2Text.text == itemName && int.TryParse(fluidSlot2Qty.text, out int f2)) total += f2;

        // 🔹 Solid
        if (solidSlot1Text.text == itemName && int.TryParse(solidSlot1Qty.text, out int s1)) total += s1;
        if (solidSlot2Text.text == itemName && int.TryParse(solidSlot2Qty.text, out int s2)) total += s2;

        return total;
    }

    // =========================================================
    // TOMBOL MERGE / REAKSI (bisa dipakai langsung juga)
    // =========================================================
    public void OnBakePressed()
    {
        int qtyH = GetQuantity("H");
        int qtyO = GetQuantity("O");
        int qtyNa = GetQuantity("Na");
        int qtyCl = GetQuantity("Cl");
        int qtyH2O = GetQuantity("H2O");

        string result = "";
        string category = "";

        if (qtyH == 2 && qtyO == 1)
        {
            result = "H2O";
            category = "Fluid";
        }
        else if (qtyH == 2)
        {
            result = "H2";
            category = "Gas";
        }
        else if (qtyO == 2)
        {
            result = "O2";
            category = "Gas";
        }
        else if (qtyNa == 1 && qtyCl == 1)
        {
            result = "NaCl";
            category = "Solid";
        }
        else if (qtyH2O == 1 && qtyNa == 1)
        {
            result = "NaOH";
            category = "Fluid";
        }
        else
        {
            resultText.text = "❌ Kombinasi tidak valid.";
            Debug.LogWarning("❌ Kombinasi bahan tidak valid.");
            return;
        }

        reactor.productName = result;
        reactor.productCategory = category;
        reactor.quantity = 1;

        if (reactor == null)
            Debug.LogError("❌ ReactorTank belum di-assign di AssemblerController!");
        else
            Debug.Log($"✅ Simpan ke reactor: {reactor.name} → {result} ({category}) qty={reactor.quantity}");

        resultText.text = $"✅ Menghasilkan {result} ({category})";
        Debug.Log($"✅ Hasil reaksi: {result} [{category}]");
    }
}
