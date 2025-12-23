using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Mengatur daftar pesanan (order) dan progress-nya.
/// Dipakai oleh OrderComputer (OrderInteract) dan OrderPanel UI.
/// </summary>
public class OrderManager : MonoBehaviour
{
    [Header("UI Referensi")]
    public TextMeshProUGUI orderTitleText;   // Teks judul order
    public TextMeshProUGUI orderStatusText;  // Status seperti “In Progress” / “Delivered”
    public Button sendButton;                // Tombol kirim produk
    public ReactorTank reactor;              // Hasil produk dari assembler

    [Header("Daftar Order")]
    public List<string> orders = new List<string>()
    {
        "H2",
        "O2",
        "H2O",
        "NaCl",
        "NaOH"
    };

    private int currentOrderIndex = 0;
    private bool isCompleted = false;

    void Start()
    {
        UpdateUI();
        if (sendButton != null)
            sendButton.onClick.AddListener(OnSendOrder);
    }

    // MENGAMBIL ORDER AKTIF SAAT INI
    public string GetCurrentOrder()
    {
        if (isCompleted || currentOrderIndex >= orders.Count)
            return "";

        return orders[currentOrderIndex];
    }

    // DIPANGGIL SAAT ORDER SELESAI
    public void CompleteOrder()
    {
        currentOrderIndex++;

        if (currentOrderIndex >= orders.Count)
        {
            isCompleted = true;
            orderTitleText.text = "✅ Semua pesanan selesai!";
            orderStatusText.text = "";
            if (sendButton != null) sendButton.interactable = false;
            return;
        }

        UpdateUI();
    }

    // UPDATE TEKS DI UI
    void UpdateUI()
    {
        string current = GetCurrentOrder();

        if (string.IsNullOrEmpty(current))
        {
            orderTitleText.text = "Tidak ada order aktif.";
            orderStatusText.text = "";
        }
        else
        {
            orderTitleText.text = "Pesanan Aktif: " + current;
            orderStatusText.text = "Status: Menunggu produk...";
        }
    }

    // DIPANGGIL DARI INTERACT (misal saat panel dibuka)
    public void UpdateOrderDisplay()
    {
        UpdateUI(); // panggil fungsi internal yang sudah ada
    }

    // DIPANGGIL SAAT TOMBOL [KIRIM PRODUK] DIKLIK
    public void OnSendOrder()
    {
        OrderNotificationUI.Instance.ShowMessage("✅ H₂ has been sent!");
        if (isCompleted)
        {
            orderStatusText.text = "Semua order telah selesai!";
            OrderNotificationUI.Instance?.ShowMessage("🎉 All orders completed!");
            return;
        }

        if (reactor == null)
        {
            orderStatusText.text = "❌ Tidak ada hasil reaksi (reactor kosong)";
            return;
        }

        string product = reactor.productName;
        string current = GetCurrentOrder();

        if (string.IsNullOrEmpty(product))
        {
            orderStatusText.text = "❌ Reactor kosong, belum ada produk!";
            return;
        }

        if (product == current)
        {
            orderStatusText.text = $"✅ Pesanan {product} berhasil dikirim!";
            Debug.Log($"Order completed: {product}");

            // 🔹 Tampilkan notifikasi di UI
            OrderNotificationUI.Instance?.ShowMessage($"✅ {product} has been sent!");

            reactor.Clear();
            CompleteOrder();
        }
        else
        {
            orderStatusText.text = $"❌ Produk salah ({product}), butuh {current}";
            OrderNotificationUI.Instance?.ShowMessage($"⚠️ Wrong product: {product}");
        }
    }
}