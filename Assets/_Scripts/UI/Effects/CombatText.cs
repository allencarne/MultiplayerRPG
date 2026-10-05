using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CombatText : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] PlayerExperience experience;

    [SerializeField] RectTransform hightRect;
    [SerializeField] RectTransform lowRect;

    [SerializeField] CombatTextSettings settings;

    public enum TextType
    {
        Hurt,
        Crit,
        Heal,
        Exp,
        Level,
        Buff,
        Debuff
    }

    public override void OnNetworkSpawn()
    {
        stats.OnHealed.AddListener(Heal);
        stats.OnHitTaken.AddListener(HitTaken);
        if (experience != null) experience.OnEXPGained.AddListener(EXP);
        if (experience != null) experience.OnLevelUp.AddListener(Level);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnHealed.RemoveListener(Heal);
        stats.OnHitTaken.RemoveListener(HitTaken);
        if (experience != null) experience.OnEXPGained.RemoveListener(EXP);
        if (experience != null) experience.OnLevelUp.RemoveListener(Level);
    }

    void Heal(float amount)
    {
        if (amount < 1) return;

        if (IsServer)
        {
            TextClientRPC(amount, false, TextType.Heal);
        }
        else
        {
            TextServerRPC(amount, false, TextType.Heal);
        }
    }

    void EXP(float amount)
    {
        if (IsServer)
        {
            TextClientRPC(amount, false, TextType.Exp);
        }
        else
        {
            TextServerRPC(amount, false, TextType.Exp);
        }
    }

    void Level()
    {
        if (IsServer)
        {
            TextClientRPC(0, true, TextType.Level);
        }
        else
        {
            TextServerRPC(0, true, TextType.Level);
        }
    }

    [ClientRpc]
    void TextClientRPC(float amount, bool isHigh, TextType type)
    {
        Vector2 spawnPosition;

        if (isHigh)
        {
            Vector2 randomOffset = Random.insideUnitCircle * settings.spawnRadius;
            spawnPosition = (Vector2)hightRect.transform.position + randomOffset;
        }
        else
        {
            Vector2 randomOffset = Random.insideUnitCircle * settings.spawnRadius;
            spawnPosition = (Vector2)lowRect.transform.position + randomOffset;
        }

        GameObject prefab = GetTextPrefab(type);
        if (prefab == null) return;

        GameObject popUp = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        TextMeshProUGUI popUpText = popUp.GetComponent<TextMeshProUGUI>();
        
        switch (type)
        {
            case TextType.Hurt: popUpText.text = amount.ToString(); break;
            case TextType.Crit: popUpText.text = amount.ToString(); break;
            case TextType.Heal: popUpText.text = amount.ToString(); break;
            case TextType.Exp: popUpText.text = $"+ {amount} EXP"; break;
            case TextType.Level: popUpText.text = "LEVEL UP"; break;
            case TextType.Buff: popUpText.text = "+Buff"; break;
            case TextType.Debuff: popUpText.text = "+DeBuff"; break;
        }
    }

    [ServerRpc]
    void TextServerRPC(float amount, bool isHigh, TextType type)
    {
        TextClientRPC(amount, isHigh, type);
    }

    GameObject GetTextPrefab(TextType type)
    {
        switch (type)
        {
            case TextType.Hurt: return settings.Hurt;
            case TextType.Crit: return settings.Crit;
            case TextType.Heal: return settings.Heal;
            case TextType.Exp: return settings.Exp;
            case TextType.Level: return settings.Level;
            case TextType.Buff: return settings.Buff;
            case TextType.Debuff: return settings.Debuff;
            default: return null;
        }
    }

    void HitTaken(int amount, HitType type)
    {
        ShowHitClientRPC(amount, type);
    }

    [ClientRpc]
    void ShowHitClientRPC(int amount, HitType type)
    {
        GameObject prefab = settings.Get(type);
        if (prefab == null) return;

        // crits go higher, everything else low
        RectTransform anchor = type == HitType.Crit ? hightRect : lowRect;
        Vector2 pos = (Vector2)anchor.position + Random.insideUnitCircle * settings.spawnRadius;

        GameObject popUp = Instantiate(prefab, pos, Quaternion.identity, transform);
        popUp.GetComponent<TextMeshProUGUI>().text = amount.ToString();
    }
}
