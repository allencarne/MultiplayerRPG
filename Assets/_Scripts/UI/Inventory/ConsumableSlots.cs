using System;
using UnityEngine;
using UnityEngine.Events;

public class ConsumableSlots : MonoBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] Inventory inventory;

    [Header("Cooldowns (seconds)")]
    [SerializeField] float potionCooldown = 180f; // 3 minutes
    [SerializeField] float foodCooldown = 300f;   // 5 minutes

    // One entry per ConsumableSlotType (Potion = 0, Food = 1)
    InventorySlotData[] slots = new InventorySlotData[2];
    float[] readyTime = new float[2]; // Time.time when each slot can be used again

    public UnityEvent OnSlotsChanged; // hook your UI up to this later

    public InventorySlotData GetSlot(ConsumableSlotType type) => slots[(int)type];

    public bool IsReady(ConsumableSlotType type) => Time.time >= readyTime[(int)type];

    public float CooldownRemaining(ConsumableSlotType type) => Mathf.Max(0f, readyTime[(int)type] - Time.time);

    public float GetCooldown(ConsumableSlotType type) => type == ConsumableSlotType.Potion ? potionCooldown : foodCooldown;

    public bool Equip(InventorySlotData newSlot)
    {
        Consumable newItem = newSlot?.item as Consumable;
        if (newItem == null) return false;

        int i = (int)newItem.SlotType;
        InventorySlotData oldSlot = slots[i];
        if (oldSlot != null && oldSlot.item == null) oldSlot = null;

        // Take the stack out of the inventory
        int invIndex = Array.IndexOf(inventory.items, newSlot);
        if (invIndex != -1)
        {
            inventory.items[invIndex] = null;
            inventory.Save.SaveInventory(null, invIndex);
        }

        if (oldSlot != null && oldSlot.item.ITEM_ID == newItem.ITEM_ID)
        {
            // Same consumable already in the slot, just add to the stack
            oldSlot.quantity += newSlot.quantity;
        }
        else
        {
            slots[i] = newSlot;

            // Different consumable was in there, send it back to the inventory
            if (oldSlot != null) inventory.AddItem(oldSlot, true);
        }

        inventory.inventoryUI.UpdateUI();
        OnSlotsChanged?.Invoke();
        return true;
    }

    public void UnEquip(ConsumableSlotType type)
    {
        int i = (int)type;
        if (slots[i] == null) return;

        if (!inventory.AddItem(slots[i], true))
        {
            Debug.Log("Inventory full — could not unequip consumable.");
            return;
        }

        slots[i] = null;
        OnSlotsChanged?.Invoke();
    }

    // Your input callback will call this later
    public bool TryConsume(ConsumableSlotType type)
    {
        int i = (int)type;
        InventorySlotData slot = slots[i];

        if (slot == null) return false;
        if (!IsReady(type)) return false;

        Consumable consumable = (Consumable)slot.item;
        if (!consumable.Apply(stats)) return false; // don't burn the cooldown if it failed

        readyTime[i] = Time.time + GetCooldown(type);

        slot.quantity--;
        if (slot.quantity <= 0) slots[i] = null;

        OnSlotsChanged?.Invoke();
        return true;
    }
}
