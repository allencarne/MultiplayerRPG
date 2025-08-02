using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.InputSystem;
using System.Collections;

public class ItemPickup : NetworkBehaviour
{
    public Item Item;
    [SerializeField] GameObject toolTip;
    [SerializeField] TextMeshProUGUI pickupText;
    [SerializeField] InputActionReference pickupAction;
    [SerializeField] GameObject itemSprite;
    [SerializeField] AnimateItem animateItem;
    bool _hasBeenPickedUp = false;
    PlayerInput playerInput;

    public void PickUp(Player player)
    {
        if (_hasBeenPickedUp) return;
        _hasBeenPickedUp = true;

        if (Item is Currency)
        {
            player.CoinCollected(Item.Quantity);
            PlayPickupEffect();
            return;
        }

        // Add Item to Inventory if we have enough space
        bool wasPickedUp = player.PlayerInventory.AddItem(Item);

        // Destroy item if it was collected
        if (wasPickedUp)
        {
            PlayPickupEffect();
        }
        else
        {
            _hasBeenPickedUp = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DespawnServerRPC()
    {
        NetworkObject.Despawn(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();
        if (!player || !player.IsLocalPlayer) return;

        // Store reference to PlayerInput
        playerInput = player.GetComponent<PlayerInput>();
        if (playerInput == null) return;

        // Show tooltip
        toolTip.SetActive(true);
        UpdatePickupText();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();
        if (!player || !player.IsLocalPlayer) return;

        toolTip.SetActive(false);
        pickupText.text = "";
    }

    private void UpdatePickupText()
    {
        if (playerInput == null || pickupAction == null)
        {
            pickupText.text = "Press Interact";
            return;
        }

        string controlScheme = playerInput.currentControlScheme;
        int bindingIndex = GetBindingIndexForCurrentScheme(controlScheme);

        string bindName = pickupAction.action.GetBindingDisplayString(bindingIndex);
        pickupText.text = $"Press <color=#00FF00>{bindName}</color> to pick up";
    }

    private int GetBindingIndexForCurrentScheme(string scheme)
    {
        for (int i = 0; i < pickupAction.action.bindings.Count; i++)
        {
            var binding = pickupAction.action.bindings[i];
            if (binding.groups.Contains(scheme))
            {
                return i;
            }
        }

        return 0; // Fallback
    }

    private void PlayPickupEffect()
    {
        toolTip.SetActive(false);
        animateItem.canAnimate = false;

        if (itemSprite == null)
        {
            DespawnImmediately();
            return;
        }

        // Optional: disable collider so player can't re-trigger it
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        StartCoroutine(PickupAnimationCoroutine());
    }

    private IEnumerator PickupAnimationCoroutine()
    {
        float duration = 0.5f;
        float time = 0f;

        Vector3 startPos = itemSprite.transform.position;
        Vector3 targetPos = startPos + new Vector3(Random.Range(-0.2f, 0.2f), 1.0f, 0f);
        Vector3 startScale = itemSprite.transform.localScale;
        Vector3 endScale = startScale * 1.3f;

        SpriteRenderer sprite = itemSprite.GetComponent<SpriteRenderer>();
        Color startColor = sprite != null ? sprite.color : Color.white;

        while (time < duration)
        {
            float t = time / duration;

            // Smooth upward arc
            itemSprite.transform.position = Vector3.Lerp(startPos, targetPos, t);
            itemSprite.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            // Fade out
            if (sprite != null)
            {
                Color c = startColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                sprite.color = c;
            }

            time += Time.deltaTime;
            yield return null;
        }

        DespawnImmediately();
    }

    private void DespawnImmediately()
    {
        if (IsServer)
        {
            NetworkObject.Despawn(true);
        }
        else
        {
            DespawnServerRPC();
        }
    }
}
