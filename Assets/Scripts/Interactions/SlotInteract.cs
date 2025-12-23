using UnityEngine;

public class SlotInteract : MonoBehaviour, IInteractable
{
    [Header("Slot Settings")]
    public string slotCategory = "Gas"; // "Gas", "Fluid", atau "Solid"
    public Transform dropPoint;         // posisi tempat item keluar

    [Header("State")]
    public string storedItemName = "";
    public bool isFilled = false;
    public GameObject storedObject;

    private bool isLocked = false;

    // INTERACT
    public void OnInteractRight() => TryInteract(DualHandPickup.RightHand);
    public void OnInteractLeft()  => TryInteract(DualHandPickup.LeftHand);

    public string GetPromptMessage()
    {
        if (isFilled)
            return $"[E] Keluarkan {slotCategory}";
        else
            return $"[E] Masukkan {slotCategory}";
    }

    // MAIN INTERACT
    void TryInteract(Transform hand)
    {
        if (isLocked) return;
        isLocked = true;

        if (isFilled)
            DropItem();
        else
            InsertItem(hand);

        Invoke(nameof(Unlock), 0.2f);
    }

    void Unlock() => isLocked = false;

    // INSERT ITEM
    void InsertItem(Transform hand)
    {
        if (isFilled || storedObject != null)
        {
            Debug.LogWarning($"❌ Slot {name} sudah berisi {storedItemName}!");
            return;
        }

        RackItem item = hand.GetComponentInChildren<RackItem>();
        if (item == null)
        {
            Debug.Log($"🖐️ Tidak ada item {slotCategory} di tangan.");
            InteractionPromptUI.Instance.Show($"Tangan tidak membawa {slotCategory.ToLower()}!");
            return;
        }

        // Pastikan item sesuai kategori
        if (!item.itemCategory.Equals(slotCategory, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogWarning($"❌ Item kategori {item.itemCategory} tidak cocok dengan slot {slotCategory}!");
            InteractionPromptUI.Instance.Show($"Item bukan {slotCategory.ToLower()}!");
            return;
        }

        storedObject = item.gameObject;
        storedItemName = item.itemName;
        isFilled = true;

        // Pindahkan item ke slot
        item.transform.SetParent(transform);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Debug.Log($"✅ {slotCategory} '{storedItemName}' dimasukkan ke slot {name}");
    }

    // DROP ITEM
    void DropItem()
    {
        if (storedObject == null)
        {
            Debug.LogWarning($"⚠️ Slot {name} kosong, reset...");
            Clear();
            return;
        }

        storedObject.transform.SetParent(null);
        storedObject.transform.position = dropPoint.position;
        storedObject.transform.rotation = dropPoint.rotation;

        Rigidbody rb = storedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Debug.Log($"🔻 {slotCategory} '{storedItemName}' dikeluarkan dari slot {name}");
        Clear();
    }

    // CLEAR SLOT
    public void Clear()
    {
        storedItemName = "";
        isFilled = false;
        storedObject = null;
    }
}
