using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerExperience : NetworkBehaviour
{
    [SerializeField] RectTransform rect;

    [SerializeField] Player player;
    [SerializeField] Image expBar;
    [SerializeField] Image expBar_Back;

    [SerializeField] GameObject levelUpParticle;
    [SerializeField] GameObject levelUpParticle_Back;
    [SerializeField] GameObject levelUpText;

    public void IncreaseEXP(float amout)
    {
        player.CurrentExperience += amout;

        if (player.CurrentExperience >= player.RequiredExperience)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        SpawnEffectClientRPC();

        player.PlayerLevel++;
        player.CurrentExperience = 0;
        player.RequiredExperience = player.RequiredExperience + 5;
    }

    [ClientRpc]
    void SpawnEffectClientRPC()
    {
        GameObject text = Instantiate(levelUpText, rect.transform.position, Quaternion.identity, rect.transform);
        GameObject effect = Instantiate(levelUpParticle, transform.position, Quaternion.identity, transform);
        GameObject effect_back = Instantiate(levelUpParticle_Back, transform.position, Quaternion.identity, transform);

        Destroy(text, 3);
        Destroy(effect, 2);
        Destroy(effect_back, 2);
    }
}
