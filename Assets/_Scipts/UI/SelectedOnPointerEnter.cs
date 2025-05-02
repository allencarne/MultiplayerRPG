using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedOnPointerEnter : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
