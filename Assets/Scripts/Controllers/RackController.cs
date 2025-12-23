using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RackController : MonoBehaviour
{
    public TMP_Text labelText;
    private string itemName;
    private string category;
    private RackInteract rack;

    public void SetupButton(RackInteract rack, string itemName, string category)
    {
        this.rack = rack;
        this.itemName = itemName;
        this.category = category;

        if (labelText != null)
            labelText.text = $"{itemName}";
    }

    public void OnClick()
    {
        rack.SpawnItem(itemName);
    }
}