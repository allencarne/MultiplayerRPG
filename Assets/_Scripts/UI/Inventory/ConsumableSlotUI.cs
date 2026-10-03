using UnityEngine;

public class ConsumableSlotUI : MonoBehaviour
{
    [SerializeField] ConsumableSlots consumableSlots;
    [SerializeField] ConsumableSlot[] slots;

    private void OnEnable()
    {
        consumableSlots.OnSlotsChanged.AddListener(Refresh);
        Refresh();
    }

    private void OnDisable()
    {
        consumableSlots.OnSlotsChanged.RemoveListener(Refresh);
    }

    public void Refresh()
    {
        foreach (ConsumableSlot slot in slots)
        {
            slot.Refresh(consumableSlots.GetSlot(slot.slotType));
        }
    }
}
