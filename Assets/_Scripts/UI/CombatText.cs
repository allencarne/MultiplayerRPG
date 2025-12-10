using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CombatText : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] PlayerExperience experience;

    [SerializeField] RectTransform hightRect;
    [SerializeField] RectTransform lowRect;

    [SerializeField] GameObject Hurt_Prefab;
    [SerializeField] GameObject Deal_Prefab;
    [SerializeField] GameObject Heal_Prefab;
    [SerializeField] GameObject Exp_Prefab;
    [SerializeField] GameObject Level_Prefab;
    [SerializeField] GameObject Buff_Prefab;
    [SerializeField] GameObject DeBuff_Prefab;

    public enum TextType
    {
        Hurt,
        Deal,
        Heal,
        Exp,
        Level,
        Buff,
        Debuff
    }

    private void Awake()
    {
        stats.OnDamaged.AddListener(Hurt);
        stats.OnHealed.AddListener(Heal);
        experience.OnEXPGained.AddListener(EXP);
        experience.OnLevelUp.AddListener(Level);
    }

    private void OnDisable()
    {
        stats.OnDamaged.RemoveListener(Hurt);
        stats.OnHealed.RemoveListener(Heal);
        experience.OnEXPGained.RemoveListener(EXP);
        experience.OnLevelUp.RemoveListener(Level);
    }

    void Hurt(float amount, bool isEnemy)
    {
        if (IsServer)
        {
            if (isEnemy)
            {
                TextClientRPC(amount, false, TextType.Deal);
            }
            else
            {
                TextClientRPC(amount, false, TextType.Hurt);
            }

        }
        else
        {
            if (isEnemy)
            {
                TextServerRPC(amount, false, TextType.Deal);
            }
            else
            {
                TextServerRPC(amount, false, TextType.Hurt);
            }
            
        }
    }

    void Heal(float amount)
    {
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
            Vector2 randomOffset = Random.insideUnitCircle * .7f;
            spawnPosition = (Vector2)hightRect.transform.position + randomOffset;
        }
        else
        {
            Vector2 randomOffset = Random.insideUnitCircle * .7f;
            spawnPosition = (Vector2)lowRect.transform.position + randomOffset;
        }

        GameObject prefab = GetTextPrefab(type);
        if (prefab == null) return;

        GameObject popUp = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        TextMeshProUGUI popUpText = popUp.GetComponent<TextMeshProUGUI>();
        
        switch (type)
        {
            case TextType.Exp: popUpText.text = $"+ {amount} EXP"; break;
            case TextType.Level: popUpText.text = "LEVEL UP"; break;
            case TextType.Buff: popUpText.text = "+ Buff"; break;
            case TextType.Debuff: popUpText.text = "+ DeBuff"; break;
        }

        popUpText.text = amount.ToString();
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
            case TextType.Hurt: return Hurt_Prefab;
            case TextType.Deal: return Deal_Prefab;
            case TextType.Heal: return Heal_Prefab;
            case TextType.Exp: return Exp_Prefab;
            case TextType.Level: return Level_Prefab;
            case TextType.Buff: return Buff_Prefab;
            case TextType.Debuff: return DeBuff_Prefab;
            default: return null;
        }
    }
}
