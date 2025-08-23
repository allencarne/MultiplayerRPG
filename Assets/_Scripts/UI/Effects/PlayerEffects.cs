using Unity.Netcode;
using UnityEngine;

public class PlayerEffects : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject skillSelect_Effect;
    [SerializeField] GameObject levelUp_Effect;
    [SerializeField] GameObject levelUpBack_Effect;

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
        GameObject effect = Instantiate(skillSelect_Effect, player.transform.position, Quaternion.identity, player.transform);
        Destroy(effect, 1);
    }

    [ClientRpc]
    public void LevelUpClientRPC()
    {
        //GameObject text = Instantiate(levelUpText, rect.transform.position, Quaternion.identity, rect.transform);
        GameObject effect = Instantiate(levelUp_Effect, player.transform.position, Quaternion.identity, player.transform);
        GameObject effect_back = Instantiate(levelUpBack_Effect, player.transform.position, Quaternion.identity, player.transform);

        //Destroy(text, 3);
        Destroy(effect, 2);
        Destroy(effect_back, 2);
    }
}
