using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DeBuffs : NetworkBehaviour, ISlowable
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] Buffs buffs;

    [SerializeField] GameObject debuffBar;
    [SerializeField] GameObject debuff_Slow;
    [SerializeField] GameObject debuff_Weakness;
    [SerializeField] GameObject debuff_Impede;
    [SerializeField] GameObject debuff_Vulnerability;
    [SerializeField] GameObject debuff_Exhaust;

    GameObject slowInstance;
    GameObject weaknessInstance;
    GameObject impedeInstance;
    GameObject vulnerabilityInstance;
    GameObject exhaustInstance;

    [HideInInspector] public int SlowStacks;
    [HideInInspector] public int WeaknessStacks;
    [HideInInspector] public int ImpedeStacks;
    [HideInInspector] public int VulnerabilityStacks;
    [HideInInspector] public int ExhaustStacks;

    [HideInInspector] public float slowPercent = 0.036f;
    [HideInInspector] public float weaknessPercent = 0.036f;
    [HideInInspector] public float impedePercent = 0.036f;
    [HideInInspector] public float vulnerabilityPercent = 0.036f;
    [HideInInspector] public float exhaustPercent = 0.036f;

    #region Slow

    public void Slow(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(SlowDuration(stacks, duration));
        }
        else
        {
            SlowDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator SlowDuration(int stacks, float duration)
    {
        SlowStacks += stacks;
        SlowStacks = Mathf.Min(SlowStacks, 25);

        if (!slowInstance) InstantiateSlowClientRPC();
        UpdateSlowUIClientRPC(SlowStacks);

        ApplySlow();

        yield return new WaitForSeconds(duration);

        SlowStacks -= stacks;
        SlowStacks = Mathf.Max(SlowStacks, 0);

        ApplySlow();

        if (SlowStacks == 0) DestroySlowClientRPC();

        UpdateSlowUIClientRPC(SlowStacks);
    }

    void ApplySlow()
    {
        float hasteMultiplier = buffs.HasteStacks * buffs.hastePercent;
        float slowMultiplier = SlowStacks * slowPercent;

        float multiplier = 1 + hasteMultiplier - slowMultiplier;

        if (player != null)
        {
            float speed = player.BaseSpeed.Value * multiplier;
            player.CurrentSpeed.Value = Mathf.Max(speed, 0.1f);
        }

        if (enemy != null)
        {
            float speed = enemy.BaseSpeed * multiplier;
            enemy.CurrentSpeed = Mathf.Max(speed, 0.1f);
        }
    }

    [ServerRpc]
    void SlowDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(SlowDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateSlowClientRPC()
    {
        slowInstance = Instantiate(debuff_Slow, debuffBar.transform);
    }

    [ClientRpc]
    void UpdateSlowUIClientRPC(int stacks)
    {
        if (slowInstance) slowInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroySlowClientRPC()
    {
        if (slowInstance) Destroy(slowInstance);
    }

    #endregion

    #region Weakness

    public void Weakness(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(WeaknessDuration(stacks, duration));
        }
        else
        {
            WeaknessDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator WeaknessDuration(int stacks, float duration)
    {
        WeaknessStacks += stacks;
        WeaknessStacks = Mathf.Min(WeaknessStacks, 25);

        if (!weaknessInstance) InstantiateWeaknessClientRPC();
        UpdateWeaknessUIClientRPC(WeaknessStacks);

        ApplyWeakness();

        yield return new WaitForSeconds(duration);

        WeaknessStacks -= stacks;
        WeaknessStacks = Mathf.Max(WeaknessStacks, 0);

        ApplyWeakness();

        if (WeaknessStacks == 0) DestroyWeaknessClientRPC();
        UpdateWeaknessUIClientRPC(WeaknessStacks);
    }

    void ApplyWeakness()
    {
        float mightMultiplier = buffs.MightStacks * buffs.mightPercent;
        float weaknessMultiplier = WeaknessStacks * weaknessPercent;

        float multiplier = 1 + mightMultiplier - weaknessMultiplier;

        if (player != null)
        {
            float damage = player.BaseDamage.Value * multiplier;
            player.CurrentDamage.Value = Mathf.Max(Mathf.RoundToInt(damage), 1);
        }

        if (enemy != null)
        {
            float damage = enemy.BaseDamage * multiplier;
            enemy.CurrentDamage = Mathf.Max(Mathf.RoundToInt(damage), 1);
        }
    }

    [ServerRpc]
    void WeaknessDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(WeaknessDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateWeaknessClientRPC()
    {
        weaknessInstance = Instantiate(debuff_Weakness, debuffBar.transform);
    }

    [ClientRpc]
    void UpdateWeaknessUIClientRPC(int stacks)
    {
        if (weaknessInstance) weaknessInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyWeaknessClientRPC()
    {
        if (weaknessInstance) Destroy(weaknessInstance);
    }

    #endregion

    #region Impede

    public void Impede(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(ImpedeDuration(stacks, duration));
        }
        else
        {
            ImpedeDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator ImpedeDuration(int stacks, float duration)
    {
        ImpedeStacks += stacks;
        ImpedeStacks = Mathf.Min(ImpedeStacks, 25);

        if (!impedeInstance) InstantiateImpedeClientRPC();
        UpdateImpedeUIClientRPC(ImpedeStacks);

        ApplyImpede();

        yield return new WaitForSeconds(duration);

        ImpedeStacks -= stacks;
        ImpedeStacks = Mathf.Max(ImpedeStacks, 0);

        ApplyImpede();

        if (ImpedeStacks == 0) DestroyImpedeClientRPC();
        UpdateImpedeUIClientRPC(ImpedeStacks);
    }

    void ApplyImpede()
    {
        float alacrityMultiplier = buffs.AlacrityStacks * buffs.alacrityPercent;
        float impedeMultiplier = ImpedeStacks * impedePercent;

        float multiplier = 1 + alacrityMultiplier - impedeMultiplier;

        if (player != null)
        {
            float cdr = player.BaseCDR.Value * multiplier;
            player.CurrentCDR.Value = Mathf.Max(cdr, 0.1f);
        }

        if (enemy != null)
        {
            float cdr = enemy.BaseCDR * multiplier;
            enemy.CurrentCDR = Mathf.Max(cdr, 0.1f);
        }
    }

    [ServerRpc]
    void ImpedeDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(ImpedeDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateImpedeClientRPC()
    {
        impedeInstance = Instantiate(debuff_Impede, debuffBar.transform);
    }

    [ClientRpc]
    void UpdateImpedeUIClientRPC(int stacks)
    {
        if (impedeInstance) impedeInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyImpedeClientRPC()
    {
        if (impedeInstance) Destroy(impedeInstance);
    }

    #endregion

    #region Vulnerability

    public void Vulnerability(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(VulnerabilityDuration(stacks, duration));
        }
        else
        {
            VulnerabilityDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator VulnerabilityDuration(int stacks, float duration)
    {
        VulnerabilityStacks += stacks;
        VulnerabilityStacks = Mathf.Min(VulnerabilityStacks, 25);

        if (!vulnerabilityInstance) InstantiateVulnerabilityClientRPC();
        UpdateVulnerabilityUIClientRPC(VulnerabilityStacks);

        ApplyVulnerability();

        yield return new WaitForSeconds(duration);

        VulnerabilityStacks -= stacks;
        VulnerabilityStacks = Mathf.Max(VulnerabilityStacks, 0);

        ApplyVulnerability();

        if (VulnerabilityStacks == 0) DestroyVulnerabilityClientRPC();
        UpdateVulnerabilityUIClientRPC(VulnerabilityStacks);
    }

    void ApplyVulnerability()
    {
        float protectionMultiplier = buffs.ProtectionStacks * buffs.protectionPercent;
        float vulnerabilityMultiplier = VulnerabilityStacks * vulnerabilityPercent;

        float multiplier = 1 + protectionMultiplier - vulnerabilityMultiplier;

        if (player != null)
        {
            float armor = player.BaseArmor.Value * multiplier;
            player.CurrentArmor.Value = Mathf.Max(armor, 0.1f);
        }

        if (enemy != null)
        {
            float armor = enemy.BaseArmor * multiplier;
            enemy.CurrentArmor = Mathf.Max(armor, 0.1f);
        }
    }

    [ServerRpc]
    void VulnerabilityDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(VulnerabilityDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateVulnerabilityClientRPC()
    {
        vulnerabilityInstance = Instantiate(debuff_Vulnerability, debuffBar.transform);
    }

    [ClientRpc]
    void UpdateVulnerabilityUIClientRPC(int stacks)
    {
        if (vulnerabilityInstance) vulnerabilityInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyVulnerabilityClientRPC()
    {
        if (vulnerabilityInstance) Destroy(vulnerabilityInstance);
    }

    #endregion

    #region Exhaust

    public void Exhaust(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(ExhaustDuration(stacks, duration));
        }
        else
        {
            ExhaustDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator ExhaustDuration(int stacks, float duration)
    {
        ExhaustStacks += stacks;
        ExhaustStacks = Mathf.Min(ExhaustStacks, 25);

        if (!exhaustInstance) InstantiateExhaustClientRPC();
        UpdateExhaustUIClientRPC(ExhaustStacks);

        ApplyExhaust();

        yield return new WaitForSeconds(duration);

        ExhaustStacks -= stacks;
        ExhaustStacks = Mathf.Max(ExhaustStacks, 0);

        ApplyExhaust();

        if (ExhaustStacks == 0) DestroyExhaustClientRPC();
        UpdateExhaustUIClientRPC(ExhaustStacks);
    }

    void ApplyExhaust()
    {
        float swiftnessMultiplier = buffs.SwiftnessStacks * buffs.swiftnessPercent;
        float exhaustMultiplier = ExhaustStacks * exhaustPercent;

        float multiplier = 1 + swiftnessMultiplier - exhaustMultiplier;

        if (player != null)
        {
            float attackspeed = player.BaseAttackSpeed.Value * multiplier;
            player.CurrentAttackSpeed.Value = Mathf.Max(attackspeed, 0.1f);
        }

        if (enemy != null)
        {
            float attackspeed = enemy.BaseAttackSpeed * multiplier;
            enemy.CurrentAttackSpeed = Mathf.Max(attackspeed, 0.1f);
        }
    }

    [ServerRpc]
    void ExhaustDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(ExhaustDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateExhaustClientRPC()
    {
        exhaustInstance = Instantiate(debuff_Exhaust, debuffBar.transform);
    }

    [ClientRpc]
    void UpdateExhaustUIClientRPC(int stacks)
    {
        if (exhaustInstance) exhaustInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyExhaustClientRPC()
    {
        if (exhaustInstance) Destroy(exhaustInstance);
    }

    #endregion
}