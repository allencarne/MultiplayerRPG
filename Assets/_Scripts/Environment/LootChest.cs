using Unity.Netcode;
using UnityEngine;

public class LootChest : NetworkBehaviour, IInteractable
{
    [SerializeField] GetPlayerReference getPlayer;

    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Sprite closedSprite;
    [SerializeField] Sprite openedSprite;

    [SerializeField] string area;
    [SerializeField] int index;
    PlayerStats PlayerStats;

    [SerializeField] Item coin;
    [SerializeField] Item[] rewards;

    [SerializeField] int coinReward;
    [SerializeField] int experienceReward;

    [SerializeField] GameObject particle;

    public string DisplayName => "Loot Chest";

    public void Initalize()
    {
        if (getPlayer != null)
        {
            PlayerStats = getPlayer.player.GetComponent<PlayerStats>();
            if (PlayerStats == null) return;

            PlayerStats.net_CharacterSlot.OnValueChanged += OnCharacterSlotChanged;
            UpdateVisual(PlayerStats.net_CharacterSlot.Value);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (PlayerStats != null && PlayerStats.net_CharacterSlot != null)
        {
            PlayerStats.net_CharacterSlot.OnValueChanged -= OnCharacterSlotChanged;
        }
    }

    private void OnCharacterSlotChanged(int previousValue, int newValue)
    {
        UpdateVisual(newValue);
    }

    public void UpdateVisual(int characterNumber)
    {
        string status = PlayerPrefs.GetString($"Character{characterNumber}_{area}_Chest_{index}", "Incomplete");

        if (status == "Completed")
        {
            sprite.sprite = openedSprite;
        }
        else
        {
            sprite.sprite = closedSprite;
        }
    }

    public void Interact(PlayerInteract player)
    {
        PlayerStats stats = player.GetComponentInParent<PlayerStats>();
        if (stats == null || !stats.IsOwner) return;

        int characterNumber = stats.net_CharacterSlot.Value;

        string status = PlayerPrefs.GetString($"Character{characterNumber}_{area}_Chest_{index}", "Incomplete");

        if (status == "Completed") return;

        PlayerPrefs.SetString($"Character{characterNumber}_{area}_Chest_{index}", "Completed");
        PlayerPrefs.Save();

        sprite.sprite = openedSprite;
        if (IsServer)
        {
            SpawnClientRPC();
        }
        else
        {
            SpawnServerRPC();
        }

        CoinReward();
        ItemReward();
        ExperienceReward();
    }

    void CoinReward()
    {
        if (getPlayer.player != null)
        {
            Inventory inventory = getPlayer.player.GetComponentInChildren<Inventory>();
            if (inventory != null) inventory.AddItem(coin,coinReward);
        }
    }

    void ItemReward()
    {
        foreach (Item reward in rewards)
        {
            if (Random.Range(0f, 100f) < reward.DropChance)
            {
                if (getPlayer.player != null)
                {
                    Inventory inventory = getPlayer.player.GetComponentInChildren<Inventory>();
                    if (inventory != null) inventory.AddItem(reward);
                }
            }
        }
    }

    public void ExperienceReward()
    {
        if (getPlayer.player != null)
        {
            PlayerExperience playerEXP = getPlayer.player.GetComponent<PlayerExperience>();
            if (playerEXP != null) playerEXP.IncreaseEXP(experienceReward);
        }
    }

    [ClientRpc]
    void SpawnClientRPC()
    {
        Instantiate(particle, transform.position, Quaternion.identity);
    }

    [ServerRpc]
    void SpawnServerRPC()
    {
        SpawnClientRPC();
    }
}
