using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject toolTip;
    public Item Item;
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (toolTip != null && Item != null)
        {
            toolTip.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toolTip != null)
        {
            toolTip.SetActive(false);
        }
    }
}
