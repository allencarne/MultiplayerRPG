using Unity.Netcode;
using UnityEngine;

public class PlayerEffects : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject skillSelect_Effect;

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
        GameObject effect = Instantiate(skillSelect_Effect, player.transform.position, Quaternion.identity);
        Destroy(effect, 1);
    }
}
