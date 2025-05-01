using System.Collections;
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

    Coroutine lerp;

    private void Start()
    {
        UpdateEXPBar(player.CurrentExperience, player.RequiredExperience);
    }

    public void IncreaseEXP(float amout)
    {
        player.CurrentExperience += amout;

        if (player.CurrentExperience >= player.RequiredExperience)
        {
            LevelUp();
        }

        UpdateEXPBar(player.CurrentExperience, player.RequiredExperience);
    }

    void LevelUp()
    {
        SpawnEffectClientRPC();

        player.PlayerLevel++;
        player.CurrentExperience -= player.RequiredExperience;
        player.RequiredExperience += 5;

        if (player.CurrentExperience >= player.RequiredExperience)
        {
            LevelUp();
        }
    }

    public void UpdateEXPBar(float currentEXP, float reqEXP)
    {
        if (reqEXP <= 0) return;

        expBar_Back.fillAmount = currentEXP / reqEXP;

        if (lerp != null)
        {
            StopCoroutine(lerp);
        }
        lerp = StartCoroutine(LerpBar(currentEXP / reqEXP));
    }

    IEnumerator LerpBar(float targetFillAmount)
    {
        float currentFillAmount = expBar.fillAmount;
        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, 5f * Time.deltaTime);
            expBar.fillAmount = currentFillAmount;
            yield return null;
        }
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
