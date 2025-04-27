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

    public int SlowStacks;
    public int WeaknessStacks;
    public int ImpedeStacks;
    public int VulnerabilityStacks;
    public int ExhaustStacks;

    public float SlowPercent = 0.03f;
    public float WeaknessPercent = .10f;
    public float ImpedePercent = .10f;
    public float VulnerabilityPercent = .10f;
    public float ExhaustPercent = .10f;


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
        float _Haste = buffs.HasteStacks * 0.03f;
        float _Slow = SlowStacks * 0.03f;

        float multiplier = 1 + _Haste - _Slow;

        if (player != null)
        {
            player.CurrentSpeed.Value = player.BaseSpeed.Value * multiplier;
        }

        if (enemy != null)
        {
            enemy.CurrentSpeed = enemy.BaseSpeed * multiplier;
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
        slowInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroySlowClientRPC()
    {
        Destroy(slowInstance);
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

        // Apply Debuff
        if (player != null) player.CurrentDamage.Value = player.BaseDamage.Value - WeaknessStacks;
        if (enemy != null) enemy.CurrentDamage = enemy.BaseDamage - WeaknessStacks;

        yield return new WaitForSeconds(duration);

        WeaknessStacks -= stacks;
        WeaknessStacks = Mathf.Max(WeaknessStacks, 0);

        // Apply Debuff
        if (player != null) player.CurrentDamage.Value = player.BaseDamage.Value - WeaknessStacks;
        if (enemy != null) enemy.CurrentDamage = enemy.BaseDamage - WeaknessStacks;

        if (WeaknessStacks == 0) DestroyWeaknessClientRPC();

        UpdateWeaknessUIClientRPC(WeaknessStacks);
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
        weaknessInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyWeaknessClientRPC()
    {
        Destroy(weaknessInstance);
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

        // Apply Debuff
        if (player != null) player.CurrentCDR.Value = player.BaseCDR.Value - ImpedeStacks;
        if (enemy != null) enemy.CurrentCDR = enemy.BaseCDR - ImpedeStacks;

        yield return new WaitForSeconds(duration);

        ImpedeStacks -= stacks;
        ImpedeStacks = Mathf.Max(ImpedeStacks, 0);

        // Apply Debuff
        if (player != null) player.CurrentCDR.Value = player.BaseCDR.Value - ImpedeStacks;
        if (enemy != null) enemy.CurrentCDR = enemy.BaseCDR - ImpedeStacks;

        if (ImpedeStacks == 0) DestroyImpedeClientRPC();

        UpdateImpedeUIClientRPC(ImpedeStacks);
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
        impedeInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyImpedeClientRPC()
    {
        Destroy(impedeInstance);
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

        UpdateVulnerabilityUIClientRPC(SlowStacks);

        // Apply Debuff
        if (player != null) player.CurrentArmor.Value = player.BaseArmor.Value - VulnerabilityStacks;
        if (enemy != null) enemy.CurrentArmor = enemy.BaseArmor - VulnerabilityStacks;

        yield return new WaitForSeconds(duration);

        VulnerabilityStacks -= stacks;
        VulnerabilityStacks = Mathf.Max(VulnerabilityStacks, 0);

        // Apply Debuff
        if (player != null) player.CurrentArmor.Value = player.BaseArmor.Value - VulnerabilityStacks;
        if (enemy != null) enemy.CurrentArmor = enemy.BaseArmor - VulnerabilityStacks;

        if (VulnerabilityStacks == 0) DestroyVulnerabilityClientRPC();

        UpdateVulnerabilityUIClientRPC(VulnerabilityStacks);
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
        vulnerabilityInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyVulnerabilityClientRPC()
    {
        Destroy(vulnerabilityInstance);
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

        // Apply Debuff
        if (player != null) player.CurrentAttackSpeed.Value = player.BaseAttackSpeed.Value - ExhaustStacks;
        if (enemy != null) enemy.CurrentAttackSpeed = enemy.BaseAttackSpeed - ExhaustStacks;

        yield return new WaitForSeconds(duration);

        ExhaustStacks -= stacks;
        ExhaustStacks = Mathf.Max(ExhaustStacks, 0);

        // Apply Debuff
        if (player != null) player.CurrentAttackSpeed.Value = player.BaseAttackSpeed.Value - ExhaustStacks;
        if (enemy != null) enemy.CurrentAttackSpeed = enemy.BaseAttackSpeed - ExhaustStacks;

        if (ExhaustStacks == 0) DestroyExhaustClientRPC();

        UpdateExhaustUIClientRPC(ExhaustStacks);
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
        exhaustInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyExhaustClientRPC()
    {
        Destroy(exhaustInstance);
    }

    #endregion
}