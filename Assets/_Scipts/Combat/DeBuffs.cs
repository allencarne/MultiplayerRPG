using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class DeBuffs : NetworkBehaviour, ISlowable
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;

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

    int slowStacks;
    int weaknessStacks;
    int impedeStacks;
    int vulnerabilityStacks;
    int exhaustStacks;

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
        slowStacks += stacks;
        slowStacks = Mathf.Min(slowStacks, 25);

        if (!slowInstance) InstantiateSlowClientRPC();

        UpdateSlowUIClientRPC(slowStacks);

        // Apply Debuff
        if (player != null) player.CurrentSpeed.Value = player.BaseSpeed.Value - slowStacks;
        if (enemy != null) enemy.CurrentSpeed = enemy.BaseSpeed - slowStacks;

        yield return new WaitForSeconds(duration);

        slowStacks -= stacks;
        slowStacks = Mathf.Max(slowStacks, 0);

        // Apply Debuff
        if (player != null) player.CurrentSpeed.Value = player.BaseSpeed.Value - slowStacks;
        if (enemy != null) enemy.CurrentSpeed = enemy.BaseSpeed - slowStacks;

        if (slowStacks == 0) DestroySlowClientRPC();

        UpdateSlowUIClientRPC(slowStacks);
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
        weaknessStacks += stacks;
        weaknessStacks = Mathf.Min(weaknessStacks, 25);

        if (!weaknessInstance) InstantiateWeaknessClientRPC();

        UpdateWeaknessUIClientRPC(weaknessStacks);

        // Apply Debuff
        if (player != null) player.CurrentDamage.Value = player.BaseDamage.Value - weaknessStacks;
        if (enemy != null) enemy.CurrentDamage = enemy.BaseDamage - weaknessStacks;

        yield return new WaitForSeconds(duration);

        weaknessStacks -= stacks;
        weaknessStacks = Mathf.Max(weaknessStacks, 0);

        // Apply Debuff
        if (player != null) player.CurrentDamage.Value = player.BaseDamage.Value - weaknessStacks;
        if (enemy != null) enemy.CurrentDamage = enemy.BaseDamage - weaknessStacks;

        if (weaknessStacks == 0) DestroyWeaknessClientRPC();

        UpdateWeaknessUIClientRPC(weaknessStacks);
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
        impedeStacks += stacks;
        impedeStacks = Mathf.Min(impedeStacks, 25);

        if (!impedeInstance) InstantiateImpedeClientRPC();

        UpdateImpedeUIClientRPC(impedeStacks);

        // Apply Debuff
        if (player != null) player.CurrentCDR.Value = player.BaseCDR.Value - impedeStacks;
        if (enemy != null) enemy.CurrentCDR = enemy.BaseCDR - impedeStacks;

        yield return new WaitForSeconds(duration);

        impedeStacks -= stacks;
        impedeStacks = Mathf.Max(impedeStacks, 0);

        // Apply Debuff
        if (player != null) player.CurrentCDR.Value = player.BaseCDR.Value - impedeStacks;
        if (enemy != null) enemy.CurrentCDR = enemy.BaseCDR - impedeStacks;

        if (impedeStacks == 0) DestroyImpedeClientRPC();

        UpdateImpedeUIClientRPC(impedeStacks);
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
        vulnerabilityStacks += stacks;
        vulnerabilityStacks = Mathf.Min(vulnerabilityStacks, 25);

        if (!vulnerabilityInstance) InstantiateVulnerabilityClientRPC();

        UpdateVulnerabilityUIClientRPC(slowStacks);

        // Apply Debuff
        if (player != null) player.CurrentArmor.Value = player.BaseArmor.Value - vulnerabilityStacks;
        if (enemy != null) enemy.CurrentArmor = enemy.BaseArmor - vulnerabilityStacks;

        yield return new WaitForSeconds(duration);

        vulnerabilityStacks -= stacks;
        vulnerabilityStacks = Mathf.Max(vulnerabilityStacks, 0);

        // Apply Debuff
        if (player != null) player.CurrentArmor.Value = player.BaseArmor.Value - vulnerabilityStacks;
        if (enemy != null) enemy.CurrentArmor = enemy.BaseArmor - vulnerabilityStacks;

        if (vulnerabilityStacks == 0) DestroyVulnerabilityClientRPC();

        UpdateVulnerabilityUIClientRPC(vulnerabilityStacks);
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
        exhaustStacks += stacks;
        exhaustStacks = Mathf.Min(exhaustStacks, 25);

        if (!exhaustInstance) InstantiateExhaustClientRPC();

        UpdateExhaustUIClientRPC(exhaustStacks);

        // Apply Debuff
        if (player != null) player.CurrentAttackSpeed.Value = player.BaseAttackSpeed.Value - exhaustStacks;
        if (enemy != null) enemy.CurrentAttackSpeed = enemy.BaseAttackSpeed - exhaustStacks;

        yield return new WaitForSeconds(duration);

        exhaustStacks -= stacks;
        exhaustStacks = Mathf.Max(exhaustStacks, 0);

        // Apply Debuff
        if (player != null) player.CurrentAttackSpeed.Value = player.BaseAttackSpeed.Value - exhaustStacks;
        if (enemy != null) enemy.CurrentAttackSpeed = enemy.BaseAttackSpeed - exhaustStacks;

        if (exhaustStacks == 0) DestroyExhaustClientRPC();

        UpdateExhaustUIClientRPC(exhaustStacks);
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