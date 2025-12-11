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

    public override void OnNetworkSpawn()
    {
        stats.OnDamaged.AddListener(Hurt);
        stats.OnHealed.AddListener(Heal);
        stats.OnDamageDealt.AddListener(Deal);
        if (experience != null) experience.OnEXPGained.AddListener(EXP);
        if (experience != null) experience.OnLevelUp.AddListener(Level);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDamaged.RemoveListener(Hurt);
        stats.OnHealed.RemoveListener(Heal);
        stats.OnDamageDealt.RemoveListener(Deal);
        if (experience != null) experience.OnEXPGained.RemoveListener(EXP);
        if (experience != null) experience.OnLevelUp.RemoveListener(Level);
    }

    void Hurt(float amount)
    {
        if (IsServer)
        {
            TextClientRPC(amount, false, TextType.Hurt);
        }
        else
        {
            TextServerRPC(amount, false, TextType.Hurt);
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

    void Deal(float amount, Vector2 position)
    {
        if (IsServer)
        {
            DealTextClientRPC(amount, TextType.Deal, position);
        }
        else
        {
            DealTextServerRPC(amount, TextType.Deal, position);
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
            case TextType.Hurt: popUpText.text = amount.ToString(); break;
            case TextType.Deal: popUpText.text = amount.ToString(); break;
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

    [ClientRpc]
    void DealTextClientRPC(float amount, TextType type, Vector2 position)
    {
        Vector2 spawnPosition;
        Vector2 headOffset = new Vector2(0, 4.0f);

        spawnPosition = position + headOffset + Random.insideUnitCircle * 0.3f;

        GameObject prefab = GetTextPrefab(type);
        if (prefab == null) return;

        GameObject popUp = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        TextMeshProUGUI popUpText = popUp.GetComponent<TextMeshProUGUI>();

        popUpText.text = amount.ToString();
    }

    [ServerRpc]
    void DealTextServerRPC(float amount, TextType type, Vector2 position)
    {
        DealTextClientRPC(amount, type, position);
    }
}
