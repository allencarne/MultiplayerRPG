using Unity.Netcode;
using UnityEngine;

public class TotemRewards : NetworkBehaviour
{
    [SerializeField] Totem totem;
    [SerializeField] ItemStatRules rules;

    [Header("Rewards")]
    public int MaxCoinReward;
    public int MaxCollectableReward;

    public int MaxExpReward;
    [SerializeField] Item coin;
    [SerializeField] Item collectable;
    [SerializeField] Item[] rewards;

    [ClientRpc]
    public void CoinRewardsClientRpc(int amount, ClientRpcParams rpcParams)
    {
        Inventory inv = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Inventory>();
        InventorySlotData coinReward = new InventorySlotData(coin, amount, ItemRarity.Common, ItemQuality.Normal);
        inv.AddItem(coinReward);
    }

    [ClientRpc]
    public void CollectableRewardsClientRpc(int amount, ClientRpcParams rpcParams)
    {
        Inventory inv = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Inventory>();
        InventorySlotData collectableReward = new InventorySlotData(collectable, amount, ItemRarity.Common, ItemQuality.Normal);
        inv.AddItem(collectableReward);
    }

    [ClientRpc]
    public void ItemRewardsClientRpc(ClientRpcParams rpcParams)
    {
        Inventory inv = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Inventory>();

        foreach (Item reward in rewards)
        {
            if (Random.Range(0f, 100f) < reward.DropChance)
            {
                InventorySlotData _reward = new InventorySlotData(reward, 1, ItemRarity.Common, ItemQuality.Normal);
                inv.AddItem(_reward);
            }
        }
    }

    public void ExperienceRewards(Player player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        PlayerExperience playerEXP = player.GetComponent<PlayerExperience>();
        if (playerEXP == null || stats == null) return;

        float expAmount = MaxExpReward * (1f + (stats.PlayerLevel.Value * 0.15f));
        int roundedEXP = Mathf.RoundToInt(expAmount);
        playerEXP.IncreaseEXP(roundedEXP);
    }

    [ClientRpc]
    public void QuestParticipationClientRPC(string id, ClientRpcParams rpcParams = default)
    {
        PlayerQuest quest = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerQuest>();
        if (quest != null)
        {
            quest.UpdateObjective(ObjectiveType.Complete, "Totem");
            quest.UpdateObjective(ObjectiveType.Complete, id);
        }
    }
}
