using Unity.Netcode;
using UnityEngine;

public class TotemRewards : NetworkBehaviour
{
    [SerializeField] Totem totem;

    [Header("Rewards")]
    public int MaxCoinReward;
    public int MaxExpReward;
    [SerializeField] Item coin;
    [SerializeField] Item[] rewards;

    [ClientRpc]
    public void CoinRewardsClientRpc(int amount, ClientRpcParams rpcParams)
    {
        Inventory inv = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Inventory>();

        inv.AddItem(coin, amount);
    }

    [ClientRpc]
    public void ItemRewardsClientRpc(ClientRpcParams rpcParams)
    {
        Inventory inv = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponentInChildren<Inventory>();

        foreach (Item reward in rewards)
        {
            if (Random.Range(0f, 100f) < reward.DropChance)
            {
                inv.AddItem(reward);
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
