using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedOnPointerEnter : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler 
{
    [Header("Rect")]
    RectTransform rectTransform;
    [SerializeField] RectTransform[] imageRectTransform;
    Vector3 originalScale;
    Vector3 selectedScale = new Vector3(1.2f, 1.2f, 1f);

    [Header("Text Color")]
    [SerializeField] TextMeshProUGUI[] textObjects;
    [SerializeField] Color selectedTextColor = Color.green;
    Color[] originalTextColors;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        textObjects = GetComponentsInChildren<TextMeshProUGUI>();
        originalTextColors = new Color[textObjects.Length];

        for (int i = 0; i < textObjects.Length; i++)
        {
            originalTextColors[i] = textObjects[i].color;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Button button = GetComponent<Button>();
        if (button.IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = selectedScale;

        if (imageRectTransform != null)
        {
            foreach (RectTransform rt in imageRectTransform)
            {
                if (rt != null) rt.localScale = selectedScale;
            }
        }

        foreach (TextMeshProUGUI text in textObjects)
        {
            if (text != null) text.color = selectedTextColor;
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ResetScale();
    }

    void ResetScale()
    {
        if (rectTransform != null)
        {
            rectTransform.localScale = originalScale;
        }

        if (imageRectTransform != null)
        {
            foreach (RectTransform rt in imageRectTransform)
            {
                if (rt != null) rt.localScale = originalScale;
            }
        }

        for (int i = 0; i < textObjects.Length; i++)
        {
            if (textObjects[i] != null) textObjects[i].color = originalTextColors[i];
        }
    }

    void OnDisable()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        ResetScale();
    }
}
