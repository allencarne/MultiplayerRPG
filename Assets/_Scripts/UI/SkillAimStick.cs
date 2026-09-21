using UnityEngine;
using UnityEngine.EventSystems;

public class SkillAimStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    [SerializeField] PlayerInputHandler input;
    [SerializeField] RectTransform stickBackground;
    [SerializeField] RectTransform stickHandle;

    [Header("Settings")]
    [SerializeField] float deadzone = 0.1f;

    bool isAiming;

    void Awake()
    {
        if (input == null) input = GetComponentInParent<PlayerInputHandler>();
        stickBackground.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isAiming) return;
        isAiming = true;

        stickBackground.gameObject.SetActive(true);
        stickHandle.anchoredPosition = Vector2.zero;
        input.SetMobileAim(Vector2.zero);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isAiming) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(stickBackground, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);

        float radius = stickBackground.rect.width * 0.5f;
        Vector2 offset = Vector2.ClampMagnitude(localPoint, radius);
        stickHandle.anchoredPosition = offset;

        Vector2 aim = offset / radius;
        if (aim.magnitude < deadzone) aim = Vector2.zero;
        input.SetMobileAim(aim);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndAim();
    }

    void OnDisable()
    {
        EndAim();
    }

    void EndAim()
    {
        if (!isAiming) return;
        isAiming = false;
        stickBackground.gameObject.SetActive(false);
        input.ClearMobileAim();
    }
}
