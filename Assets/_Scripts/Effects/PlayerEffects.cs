using Unity.Netcode;
using UnityEngine;

public class PlayerEffects : NetworkBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] PlayerExperience exp;

    [SerializeField] GameObject spawn_Effect;
    [SerializeField] GameObject skillSelect_Effect;
    [SerializeField] GameObject levelUp_Effect;

    public override void OnNetworkSpawn()
    {
        SpawnClientRPC();
        exp.OnLevelUp.AddListener(LevelUpClientRPC);
    }

    public override void OnNetworkDespawn()
    {
        exp.OnLevelUp.RemoveListener(LevelUpClientRPC);
    }

    [ClientRpc]
    void SpawnClientRPC()
    {
        Instantiate(spawn_Effect, transform.position, transform.rotation);
    }

    public void SkillSelect()
    {
        if (!IsOwner) return;
        SkillSelectServerRPC();
    }

    [ServerRpc]
    void SkillSelectServerRPC()
    {
        SkillSelectClientRPC();
    }

    [ClientRpc]
    void SkillSelectClientRPC()
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
