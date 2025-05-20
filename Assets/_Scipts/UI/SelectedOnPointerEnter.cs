using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedOnPointerEnter : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
{
    RectTransform rectTransform;
    [SerializeField] RectTransform[] imageRectTransform;
    Vector3 originalScale;
    Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Button button = GetComponent<Button>();
        if (button.IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = selectedScale;

        if (imageRectTransform != null)
        {
            foreach (RectTransform rt in imageRectTransform)
            {
                if (rt != null)
                    rt.localScale = selectedScale;
            }
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        rectTransform.localScale = originalScale;

        if (imageRectTransform != null)
        {
            foreach (RectTransform rt in imageRectTransform)
            {
                if (rt != null)
                    rt.localScale = originalScale;
            }
        }
    }
}
