using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedOnPointerEnter : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Button button = GetComponent<Button>();
        if (button.IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
