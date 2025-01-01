using UnityEngine;
using TMPro;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] ItemPickup itemPickup;

    [SerializeField] GameObject gemSlotsImage;
    [SerializeField] TextMeshProUGUI itemInfo_Text;


    private void Start()
    {
        if (itemPickup.Item != null)
        {
            // If Item is Equipment
            if (itemPickup.Item is Equipment equipment)
            {
                gemSlotsImage.SetActive(true);
                Debug.Log(itemPickup.Item + "Is Equipment");
            }

            // If Item is Weapon
            if (itemPickup.Item is Weapon weapon)
            {
                gemSlotsImage.SetActive(true);
                Debug.Log(itemPickup.Item + "Is Weapon");
            }

            if (itemPickup.Item.IsCurrency)
            {
                gemSlotsImage.SetActive(false);
                Debug.Log(itemPickup.Item + "Is Currency");
            }

            itemInfo_Text.text = itemPickup.Item.Prefab.name;
        }
    }
}
