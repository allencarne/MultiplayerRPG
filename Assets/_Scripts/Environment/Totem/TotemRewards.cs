using System.Collections.Generic;
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

    public void ItemRewards(ClientRpcParams rpcParams)
    {
        if (!IsServer) return;

        for (int i = 0; i < rewards.Length; i++)
        {
            Item reward = rewards[i];
            if (Random.Range(0f, 100f) < reward.DropChance)
            {
                InventorySlotData slot = new InventorySlotData(reward, 1, ItemRarity.Common, ItemQuality.Normal);

                // Only equipment needs rolled rarity/quality/modifiers
                if (reward is Equipment)
                {
                    rules.RollStats(slot);
                }

                GiveItemRewardClientRpc(i, slot.quantity, slot.rarity, slot.quality, slot.modifiers.ToArray(), rpcParams);
            }
        }
    }

    [ClientRpc]
    void GiveItemRewardClientRpc(int rewardIndex, int quantity, ItemRarity rarity, ItemQuality quality, StatModifier[] modifiers, ClientRpcParams rpcParams)
    {
        Item item = rewards[rewardIndex];
        Inventory inv = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Inventory>();
        InventorySlotData slot = new InventorySlotData(item, quantity, rarity, quality, new List<StatModifier>(modifiers));
        inv.AddItem(slot);
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
