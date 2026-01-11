using Unity.Netcode;
using UnityEngine;

public class PlayerEffects : NetworkBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerExperience exp;
    [SerializeField] AttributePoints ap;
    [SerializeField] SkillPanel skillPanel;

    [SerializeField] GameObject spawn_Effect;
    [SerializeField] GameObject skillSelect_Effect;
    [SerializeField] GameObject levelUp_Effect;

    public override void OnNetworkSpawn()
    {
        SpawnClientRPC();
        exp.OnLevelUp.AddListener(LevelUpClientRPC);
        ap.OnStatsApplied.AddListener(PowerUpClientRPC);
        skillPanel.OnSkillSelected.AddListener(PowerUpClientRPC);
        stats.OnAPGained.AddListener(APGainedClientRPC);
    }

    public override void OnNetworkDespawn()
    {
        exp.OnLevelUp.RemoveListener(LevelUpClientRPC);
        ap.OnStatsApplied.RemoveListener(PowerUpClientRPC);
        skillPanel.OnSkillSelected.RemoveListener(PowerUpClientRPC);
        stats.OnAPGained.RemoveListener(APGainedClientRPC);
    }

    [ClientRpc]
    void SpawnClientRPC()
    {
        Instantiate(spawn_Effect, transform.position, transform.rotation);
    }

    [ClientRpc]
    void APGainedClientRPC()
    {
        Instantiate(spawn_Effect, transform.position, transform.rotation);
    }

    [ClientRpc]
    void PowerUpClientRPC()
    {
        GameObject effect = Instantiate(skillSelect_Effect, transform.position, Quaternion.identity, transform);
        Destroy(effect, 1);
    }

    [ClientRpc]
    public void LevelUpClientRPC()
    {
        GameObject effect = Instantiate(levelUp_Effect, transform.position, Quaternion.identity, transform);
        Destroy(effect, 2);
    }
}
