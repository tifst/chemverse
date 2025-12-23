using UnityEngine;
using System.Collections.Generic;

public class RackInteract : MonoBehaviour, IInteractable
{
    [Header("Rack Configuration")]
    public string rackCategory = "Gas"; // "Gas", "Fluid", atau "Solid"
    public Transform spawnPoint;
    public GameObject selectUIPanel;
    public Transform buttonContainer; // tempat tombol muncul (Vertical Layout Group)

    [Header("Item Prefabs (campur semua boleh)")]
    public List<RackItem> itemPrefabs; // berisi semua item: gas, solid, fluid

    [Header("Button Prefab")]
    public GameObject rackButtonPrefab; // prefab tombol UI Button

    private Dictionary<string, RackItem> prefabLookup = new();
    private Dictionary<string, GameObject> buttonCache = new();
    private Transform pendingHand;
    public static bool UIActive = false;

    // ============================================
    void Start()
    {
        // Siapkan lookup hanya untuk kategori ini
        foreach (var item in itemPrefabs)
        {
            if (item.itemCategory != rackCategory) continue; // ⬅️ FILTER kategori
            if (!prefabLookup.ContainsKey(item.itemName))
                prefabLookup.Add(item.itemName, item);
        }

        GenerateButtons();
    }

    // ============================================
    // BUAT TOMBOL SEKALI, HANYA UNTUK KATEGORI INI
    void GenerateButtons()
    {
        if (buttonContainer == null)
        {
            Debug.LogWarning($"⚠️ ButtonContainer belum di-assign di Rack: {rackCategory}");
            return;
        }

        foreach (var item in itemPrefabs)
        {
            if (item.itemCategory != rackCategory) continue; // ⬅️ hanya kategori cocok

            if (buttonCache.ContainsKey(item.itemName))
                continue;

            GameObject btn = Instantiate(rackButtonPrefab, buttonContainer, false);
            btn.name = $"Button_{rackCategory}_{item.itemName}";

            RackController ctrl = btn.GetComponent<RackController>();
            ctrl.SetupButton(this, item.itemName, rackCategory);

            buttonCache[item.itemName] = btn;
        }

        Debug.Log($"✅ {buttonCache.Count} tombol dibuat untuk Rack {rackCategory}");
    }

    // ============================================
    public string GetPromptMessage() => $"[E] Buka Rak {rackCategory}";

    public void OnInteractRight()
    {
        if (HandIsFull(DualHandPickup.RightHand)) return;
        ShowUI(DualHandPickup.RightHand);
    }

    public void OnInteractLeft()
    {
        if (HandIsFull(DualHandPickup.LeftHand)) return;
        ShowUI(DualHandPickup.LeftHand);
    }

    bool HandIsFull(Transform hand)
    {
        if (hand.GetComponentInChildren<RackItem>() != null)
        {
            InteractionPromptUI.Instance.Show("Tangan sudah memegang item!");
            return true;
        }
        return false;
    }

    // ============================================
    void ShowUI(Transform hand)
    {
        if (UIActive) return;

        pendingHand = hand;
        selectUIPanel.SetActive(true);
        UIActive = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.Hide();
    }

    // ============================================
    // DIPANGGIL DARI RACKCONTROLLER (tombol)
    public void SpawnItem(string itemName)
    {
        if (pendingHand == null) return;

        if (!prefabLookup.ContainsKey(itemName))
        {
            Debug.LogError("❌ Tidak ada prefab untuk item: " + itemName);
            return;
        }

        RackItem prefab = prefabLookup[itemName];
        RackItem spawned = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        spawned.itemName = itemName;
        spawned.itemCategory = rackCategory;

        Rigidbody rb = spawned.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        spawned.tag = "Equipment";

        spawned.transform.SetParent(pendingHand, false);
        spawned.transform.localPosition = Vector3.zero;
        spawned.transform.localRotation = Quaternion.identity;

        CloseRack();
        pendingHand = null;
    }

    // ============================================
    public void CloseRack()
    {
        selectUIPanel.SetActive(false);
        UIActive = false;

        if (InteractionPromptUI.Instance != null)
            InteractionPromptUI.Instance.Show("");

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}