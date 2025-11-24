using Unity.Netcode;
using UnityEngine;

public class TotemRewards : NetworkBehaviour
{
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
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;

            GameObject instance = Instantiate(coin.Prefab, randomPos, Quaternion.identity);
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
                Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * 2f;

                GameObject instance = Instantiate(reward.Prefab, randomPos, Quaternion.identity);
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

    public void QuestParticipation(Player player, string id)
    {
        PlayerQuest quest = player.GetComponent<PlayerQuest>();
        if (quest != null)
        {
            quest.UpdateObjective(ObjectiveType.Complete, "Totem");
            quest.UpdateObjective(ObjectiveType.Complete, id);
        }
    }
}
