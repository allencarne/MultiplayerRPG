using Unity.Netcode;
using UnityEngine;

public class LootChest : NetworkBehaviour, IInteractable
{
    [SerializeField] GetPlayerReference getPlayer;

    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Sprite closedSprite;
    [SerializeField] Sprite openedSprite;

    [SerializeField] SpriteRenderer miniMapIcon;
    [SerializeField] Sprite closedIcon;
    [SerializeField] Sprite openedIcon;

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
            miniMapIcon.sprite = openedIcon;
        }
        else
        {
            sprite.sprite = closedSprite;
            miniMapIcon.sprite = closedIcon;
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
        miniMapIcon.sprite = openedIcon;

        Instantiate(particle, transform.position, Quaternion.identity);
        ulong ownerClientId = stats.OwnerClientId;

        if (IsServer)
        {
            SpawnClientRPC(ownerClientId);
        }
        else
        {
            SpawnServerRPC(ownerClientId);
        }

        CoinReward();
        ItemReward();
        ExperienceReward();
    }

    void CoinReward()
    {
        if (getPlayer.player != null)
        {
            PlayerStats stats = getPlayer.player.GetComponent<PlayerStats>();
            Inventory inventory = getPlayer.player.GetComponentInChildren<Inventory>();

            if (inventory != null && stats != null)
            {
                float coinAmount = coinReward * (1f + (stats.PlayerLevel.Value * 0.15f));
                int roundedCoin = Mathf.RoundToInt(coinAmount);
                inventory.AddItem(coin, roundedCoin);
            }
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
            PlayerStats stats = getPlayer.player.GetComponent<PlayerStats>();
            PlayerExperience playerEXP = getPlayer.player.GetComponent<PlayerExperience>();

            if (playerEXP != null && stats != null)
            {
                float expAmount = experienceReward * (1f + (stats.PlayerLevel.Value * 0.15f));
                int roundedEXP = Mathf.RoundToInt(expAmount);
                playerEXP.IncreaseEXP(roundedEXP);
            }
        }
    }

    [ClientRpc]
    void SpawnClientRPC(ulong excludeClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == excludeClientId) return;
        Instantiate(particle, transform.position, Quaternion.identity);
    }

    [ServerRpc(RequireOwnership = false)]
    void SpawnServerRPC(ulong excludeClientId)
    {
        SpawnClientRPC(excludeClientId);
    }
}
