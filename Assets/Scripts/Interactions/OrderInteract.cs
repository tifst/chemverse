using UnityEngine;

public class OrderInteract : MonoBehaviour, IInteractable
{
    [Header("UI Panel Order")]
    public GameObject orderPanel;              // Panel UI order
    public OrderManager orderManager;    // Script pengatur isi teks di panel

    public static bool UIActive = false;

    //  INTERAKSI PLAYER
    public void OnInteractRight() => ShowOrderPanel();
    public void OnInteractLeft()  => ShowOrderPanel();

    public string GetPromptMessage()
    {
        return "[E] Lihat Pesanan";
    }

    //  BUKA PANEL PESANAN
    void ShowOrderPanel()
    {
        if (UIActive) return;

         if (InteractionPromptUI.Instance != null)
        InteractionPromptUI.Instance.Hide();

        orderPanel.SetActive(true);
        UIActive = true;

        // Update UI status order saat panel dibuka
        if (orderManager != null)
            orderManager.UpdateOrderDisplay();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        InteractionPromptUI.Instance.Hide();
    }

    //  DIPANGGIL DARI BUTTON [KIRIM PRODUK]
    public void OnSendButtonPressed()
    {
        if (orderManager != null)
            orderManager.OnSendOrder();
    }

    //  DIPANGGIL DARI BUTTON [CLOSE]
    public void ClosePanel()
    {
        orderPanel.SetActive(false);
        UIActive = false;

        if (InteractionPromptUI.Instance != null)
        InteractionPromptUI.Instance.Show("");
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}