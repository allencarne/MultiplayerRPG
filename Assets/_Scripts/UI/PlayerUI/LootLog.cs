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

    void AddLoot(Item item, int quantity)
    {
        GameObject loot = Instantiate(prefab_Loot, transform);

        GetSprite(item, loot);
        GetName(item, loot);
        GetStack(quantity, loot);

        Destroy(loot, 5);
    }

    void GetSprite(Item item, GameObject loot)
    {
        Image icon = loot.GetComponentInChildren<Image>();
        if (icon != null) icon.sprite = item.Icon;
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
