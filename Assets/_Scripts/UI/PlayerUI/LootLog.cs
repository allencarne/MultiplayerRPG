using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootLog : MonoBehaviour
{
    [SerializeField] Inventory inventory;
    [SerializeField] GameObject prefab_Loot;

    private void OnEnable()
    {
        inventory.OnItemAdded.AddListener(AddLoot);
    }

    private void OnDisable()
    {
        inventory.OnItemAdded.RemoveListener(AddLoot);
    }

    void AddLoot(InventorySlotData slotData)
    {
        GameObject loot = Instantiate(prefab_Loot, transform);

        GetSprite(slotData, loot);
        GetName(slotData.item, loot);
        GetStack(slotData.quantity, loot);

        Destroy(loot, 5);
    }

    void GetSprite(InventorySlotData slotData, GameObject loot)
    {
        Image icon = loot.transform.Find("ItemIcon").GetComponent<Image>();
        if (icon != null) icon.sprite = slotData.item.Icon;

        Image background = loot.transform.Find("ItemBackground").GetComponent<Image>();
        if (background != null) background.color = slotData.item.GetRarityColor(slotData.rarity);
    }

    void GetName(Item item, GameObject loot)
    {
        TextMeshProUGUI text = loot.transform.Find("Name_Text").GetComponent<TextMeshProUGUI>();
        if (text != null) text.text = item.name;
    }

    void GetStack(int quantity, GameObject loot)
    {
        TextMeshProUGUI text = loot.transform.Find("Amount_Text").GetComponent<TextMeshProUGUI>();

        if (quantity <= 1)
        {
            if (text != null) text.text = "";
        }
        else
        {
            if (text != null) text.text = quantity.ToString();
        }
    }
}
