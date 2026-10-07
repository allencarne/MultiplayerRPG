using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class CombatText : NetworkBehaviour
{
    public enum SelfText { Heal, Exp, Level, Buff, Debuff }

    [SerializeField] CharacterStats stats;
    [SerializeField] PlayerExperience experience;
    [SerializeField] CombatTextSettings settings;

    [FormerlySerializedAs("hightRect")][SerializeField] RectTransform topRect;
    [FormerlySerializedAs("lowRect")][SerializeField] RectTransform bottomRect;

    bool IsPlayerCharacter => stats is PlayerStats;

    public override void OnNetworkSpawn()
    {
        stats.OnHealed.AddListener(Heal);
        if (IsServer) stats.OnHitTaken.AddListener(HitTaken);
        if (experience != null)
        {
            experience.OnEXPGained.AddListener(Exp);
            experience.OnLevelUp.AddListener(Level);
        }
    }

    public override void OnNetworkDespawn()
    {
        stats.OnHealed.RemoveListener(Heal);
        stats.OnHitTaken.RemoveListener(HitTaken);
        if (experience != null)
        {
            experience.OnEXPGained.RemoveListener(Exp);
            experience.OnLevelUp.RemoveListener(Level);
        }
    }

    void HitTaken(int amount, HitType type, NetworkObject attacker)
    {
        int skin = 0;
        bool fromEnemy = false;

        // Bleed passes self as attacker, so only look at a different attacker
        if (attacker != null && attacker != NetworkObject)
        {
            fromEnemy = attacker.GetComponent<Enemy>() != null;

            PlayerStats attackerStats = attacker.GetComponent<PlayerStats>();
            if (attackerStats != null) skin = attackerStats.net_DamageSkin.Value;
        }

        ShowHitClientRpc(amount, type, (byte)skin, fromEnemy);
    }

    [ClientRpc]
    void ShowHitClientRpc(int amount, HitType type, byte skin, bool fromEnemy)
    {
        bool iAmTheVictim = IsPlayerCharacter && IsOwner;

        // Bottom for: me being hit, bleed, and anything an enemy does to anyone.
        // Top only for player/NPC damage landing on someone else.
        bool useBottom = iAmTheVictim || type == HitType.Bleed || fromEnemy;

        if (iAmTheVictim) skin = 0; // taking damage always uses the default look

        Spawn(settings.GetHit(type, skin), useBottom ? bottomRect : topRect, amount.ToString());
    }

    void Heal(float amount) { if (amount >= 1) Self(SelfText.Heal, amount); }
    void Exp(float amount) => Self(SelfText.Exp, amount);
    void Level() => Self(SelfText.Level, 0);
    public void ShowBuff() => Self(SelfText.Buff, 0);
    public void ShowDebuff() => Self(SelfText.Debuff, 0);

    void Self(SelfText type, float amount)
    {
        if (IsServer) SelfClientRpc(type, amount);
        else if (IsOwner) SelfServerRpc(type, amount);
    }

    [ServerRpc]
    void SelfServerRpc(SelfText type, float amount) => SelfClientRpc(type, amount);

    [ClientRpc]
    void SelfClientRpc(SelfText type, float amount)
    {
        bool ownerOnly = type != SelfText.Heal;
        if (ownerOnly && IsPlayerCharacter && !IsOwner) return;

        switch (type)
        {
            case SelfText.Heal: Spawn(settings.Heal, bottomRect, amount.ToString()); break;
            case SelfText.Exp: Spawn(settings.Exp, bottomRect, $"+ {amount} EXP"); break;
            case SelfText.Level: Spawn(settings.Level, topRect, "LEVEL UP"); break;
            case SelfText.Buff: Spawn(settings.Buff, bottomRect, "+Buff"); break;
            case SelfText.Debuff: Spawn(settings.Debuff, bottomRect, "+DeBuff"); break;
        }
    }

    void Spawn(GameObject prefab, RectTransform anchor, string text)
    {
        if (prefab == null) return;
        Vector2 pos = (Vector2)anchor.position + Random.insideUnitCircle * settings.spawnRadius;
        GameObject popUp = Instantiate(prefab, pos, Quaternion.identity, transform);
        popUp.GetComponent<TextMeshProUGUI>().text = text;
    }
}
