using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ItemPickup : NetworkBehaviour
{
    [HideInInspector] public SpawnItem Manager;
    [HideInInspector] public Transform SpawnPoint;
    [SerializeField] ItemStatGenerator generator;

    [SerializeField] TextMeshProUGUI quantityText;
    [SerializeField] Animator animator;

    public event System.Action<ItemPickup> Despawned;
    public Item Item => generator.Item;
    bool _hasBeenPickedUp = false;

    public override void OnNetworkSpawn()
    {
        generator.net_Quantity.OnValueChanged += OnQuantityChanged;
        UpdateQuantityUI(generator.net_Quantity.Value);
    }

    public override void OnNetworkDespawn()
    {
        generator.net_Quantity.OnValueChanged -= OnQuantityChanged;
        Despawned?.Invoke(this);
    }

    private void Start()
    {
        if (IsServer && Manager == null)
        {
            // Despawn the Item if not picked up
            StartCoroutine(DespawnOverTime(180));
        }
    }

    public void PickUp(Player player)
    {
        if (_hasBeenPickedUp) return;
        _hasBeenPickedUp = true;

        bool wasPickedUp = player.PlayerInventory.AddItem(GetSlotData());

        if (wasPickedUp)
        {
            PlayPickupEffect();
        }
        else
        {
            _hasBeenPickedUp = false;
        }
    }

    private void PlayPickupEffect()
    {
        animator.Play("Anim_Item_Pickup");

        Collider2D collider = GetComponent<Collider2D>();
        if (collider) collider.enabled = false;

        if (!IsServer) PlayPickupAnimationServerRpc();

        StartCoroutine(DespawnOverTime(.6f));
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void PlayPickupAnimationServerRpc()
    {
        animator.Play("Anim_Item_Pickup");
    }

    private void OnQuantityChanged(int previousValue, int newValue)
    {
        UpdateQuantityUI(newValue);
    }

    private void UpdateQuantityUI(int quantity)
    {
        if (quantity > 1)
        {
            quantityText.text = quantity.ToString();
        }
        else
        {
            quantityText.text = "";
        }
    }

    IEnumerator DespawnOverTime(float time)
    {
        yield return new WaitForSeconds(time);

        if (IsServer)
        {
            if (Manager != null) Manager.AddPosition(SpawnPoint);
            NetworkObject.Despawn(true);
        }
        else
        {
            DespawnServerRPC();
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void DespawnServerRPC()
    {
        if (Manager != null) Manager.AddPosition(SpawnPoint);
        NetworkObject.Despawn(true);
    }

    public InventorySlotData GetSlotData()
    {
        return new InventorySlotData(generator.Item, generator.net_Quantity.Value, generator.net_ItemRarity.Value, generator.net_ItemQuality.Value, generator.GetRolledModifiers());
    }
}
