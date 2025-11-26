using Unity.Netcode;
using UnityEngine;

public class TotemRewards : NetworkBehaviour
{
    [SerializeField] Totem totem;

    [Header("Rewards")]
    public int Coins;
    public int MaxCoinReward;
    [SerializeField] Item coin;
    [SerializeField] Item[] rewards;

    public void CoinReward()
    {
        int amountOfItems = Random.Range(1, Coins + 1);

        for (int i = 0; i < amountOfItems; i++)
        {
            GameObject instance = Instantiate(coin.Prefab, totem.GetRandomPoint(2), Quaternion.identity);
            instance.GetComponent<NetworkObject>().Spawn();

            ItemPickup pickup = instance.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                pickup.Quantity = Random.Range(0, MaxCoinReward);
            }
        }
    }

    public void ItemReward()
    {
        foreach (Item reward in rewards)
        {
            if (Random.Range(0f, 100f) < reward.DropChance)
            {
                GameObject instance = Instantiate(reward.Prefab, totem.GetRandomPoint(2), Quaternion.identity);
                instance.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    public void ExperienceRewards(Player player)
    {
        PlayerExperience playerEXP = player.GetComponent<PlayerExperience>();
        if (playerEXP != null)
        {
            playerEXP.IncreaseEXP(3);
        }
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
