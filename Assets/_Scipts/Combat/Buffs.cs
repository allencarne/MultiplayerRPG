using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buffs : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    [SerializeField] DeBuffs deBuffs;

    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Phasing;
    [SerializeField] GameObject buff_Immune;
    [SerializeField] GameObject buff_Immovable;
    [SerializeField] GameObject buff_Haste;
    [SerializeField] GameObject buff_Might;
    [SerializeField] GameObject buff_Alacrity;
    [SerializeField] GameObject buff_Protection;
    [SerializeField] GameObject buff_Swiftness;

    GameObject phasingInstance;
    GameObject immuneInstance;
    GameObject immovableInstance;
    GameObject hasteInstance;
    GameObject mightInstance;
    GameObject alacrityInstance;
    GameObject protectionInstance;
    GameObject switnessInstance;

    public bool IsPhasing;
    public bool IsImmune;
    public bool IsImmovable;

    float phasingTime;
    float immuneTime;
    float immovableTime;

    [HideInInspector] public int HasteStacks;
    [HideInInspector] public int MightStacks;
    [HideInInspector] public int AlacrityStacks;
    [HideInInspector] public int ProtectionStacks;
    [HideInInspector] public int SwiftnessStacks;

    [HideInInspector] public float hastePercent = 0.036f;
    [HideInInspector] public float mightPercent = 0.036f;
    [HideInInspector] public float alacrityPercent = 0.036f;
    [HideInInspector] public float protectionPercent = 0.036f;
    [HideInInspector] public float swiftnessPercent = 0.036f;

    private Coroutine phasingCoroutine;
    private Coroutine immuneCoroutine;
    private Coroutine immovableCoroutine;

    #region Phasing

    public void Phasing(float duration)
    {
        if (IsServer)
        {
            AddPhasingTime(duration);
        }
        else
        {
            PhasingDurationServerRPC(duration);
        }
    }

    private void AddPhasingTime(float duration)
    {
        phasingTime += duration;

        if (phasingCoroutine == null) phasingCoroutine = StartCoroutine(PhasingDuration());
    }

    private IEnumerator PhasingDuration()
    {
        PhasingClientRPC(true);

        InstantiatePhasingClientRPC();

        while (phasingTime > 0f)
        {
            phasingTime -= Time.deltaTime;
            yield return null;
        }

        PhasingClientRPC(false);

        if (phasingInstance) DestroyPhasingClientRPC();

        phasingCoroutine = null;
    }

    [ServerRpc]
    void PhasingDurationServerRPC(float duration)
    {
        AddPhasingTime(duration);
    }

    [ClientRpc]
    void PhasingClientRPC(bool isPhasing)
    {
        IsPhasing = isPhasing;
        Physics2D.IgnoreLayerCollision(6, 7, isPhasing); // Players ignore enemies
        Physics2D.IgnoreLayerCollision(6, 6, isPhasing); // Players ignore players
        Physics2D.IgnoreLayerCollision(7, 7, isPhasing); // Enemies ignore enemies
    }

    [ClientRpc]
    void InstantiatePhasingClientRPC()
    {
        if (!phasingInstance)
        {
            phasingInstance = Instantiate(buff_Phasing, buffBar.transform);
        }

        StatusEffects _UI = phasingInstance.GetComponent<StatusEffects>();

        if (_UI != null)
        {
            _UI.Initialize(phasingTime);
        }
    }

    [ClientRpc]
    void DestroyPhasingClientRPC()
    {
        Destroy(phasingInstance);
    }

    #endregion

    #region Immune

    public void Immunity(float duration)
    {
        if (IsServer)
        {
            AddImmuneTime(duration);
        }
        else
        {
            ImmunityDurationServerRPC(duration);
        }
    }

    void AddImmuneTime(float duration)
    {
        immuneTime += duration;

        if (immuneCoroutine == null) immuneCoroutine = StartCoroutine(ImmuneDuration());
    }

    IEnumerator ImmuneDuration()
    {
        ImmuneClientRPC(true);

        if (!immuneInstance) InstantiateImmuneClientRPC();

        while (immuneTime > 0f)
        {
            immuneTime -= Time.deltaTime;
            yield return null;
        }

        if (immuneInstance) DestroyImmuneClientRPC();

        ImmuneClientRPC(false);

        immuneCoroutine = null;
    }

    [ServerRpc]
    void ImmunityDurationServerRPC(float duration)
    {
        AddImmuneTime(duration);
    }

    [ClientRpc]
    void ImmuneClientRPC(bool _isImmune)
    {
        IsImmune = _isImmune;
    }

    [ClientRpc]
    void InstantiateImmuneClientRPC()
    {
        immuneInstance = Instantiate(buff_Immune, buffBar.transform);
    }

    [ClientRpc]
    void DestroyImmuneClientRPC()
    {
        Destroy(immuneInstance);
    }

    #endregion

    #region Immovable

    public void Immoveable(float duration)
    {
        if (IsServer)
        {
            AddImmovableTime(duration);
        }
        else
        {
            ImmovableDurationServerRPC(duration);
        }
    }

    void AddImmovableTime(float duration)
    {
        immovableTime += duration;

        if (immovableCoroutine == null) immovableCoroutine = StartCoroutine(ImmovableDuration());
    }

    IEnumerator ImmovableDuration()
    {
        ImmovableClientRPC(true);

        if (!immovableInstance)
        {
            InstantiateImmovableClientRPC();
        }

        while (immovableTime > 0f)
        {
            immovableTime -= Time.deltaTime;
            yield return null;
        }

        if (immovableInstance)
        {
            DestroyImmovableClientRPC();
        }

        ImmovableClientRPC(false);

        immovableCoroutine = null;
    }

    [ServerRpc]
    void ImmovableDurationServerRPC(float duration)
    {
        AddImmovableTime(duration);
    }

    [ClientRpc]
    void ImmovableClientRPC(bool _immovable)
    {
        IsImmovable = _immovable;
    }

    [ClientRpc]
    void InstantiateImmovableClientRPC()
    {
        immovableInstance = Instantiate(buff_Immovable, buffBar.transform);
    }

    [ClientRpc]
    void DestroyImmovableClientRPC()
    {
        Destroy(immovableInstance);
    }

    #endregion

    #region Haste

    public void Haste(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(HasteDuration(stacks, duration));
        }
        else
        {
            HasteDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator HasteDuration(int stacks, float duration)
    {
        HasteStacks += stacks;
        HasteStacks = Mathf.Min(HasteStacks, 25);

        if (!hasteInstance) InstantiateHasteClientRPC();
        UpdateHasteUIClientRPC(HasteStacks);

        ApplyHaste();

        yield return new WaitForSeconds(duration);

        HasteStacks -= stacks;
        HasteStacks = Mathf.Max(HasteStacks, 0);

        ApplyHaste();

        if (HasteStacks == 0) DestroyHasteClientRPC();
        UpdateHasteUIClientRPC(HasteStacks);
    }

    void ApplyHaste()
    {
        float hasteMultiplier = HasteStacks * hastePercent;
        float slowMultiplier = deBuffs.SlowStacks * deBuffs.slowPercent;

        float multiplier = 1 + hasteMultiplier - slowMultiplier;

        if (player != null) player.CurrentSpeed.Value = player.BaseSpeed.Value * multiplier;
        if (enemy != null) enemy.CurrentSpeed = enemy.BaseSpeed * multiplier;
    }

    [ServerRpc]
    void HasteDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(HasteDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateHasteClientRPC()
    {
        hasteInstance = Instantiate(buff_Haste, buffBar.transform);
    }

    [ClientRpc]
    void UpdateHasteUIClientRPC(int stacks)
    {
        if (hasteInstance) hasteInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyHasteClientRPC()
    {
        if (hasteInstance) Destroy(hasteInstance);
    }

    #endregion

    #region Might

    public void Might(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(MightDuration(stacks, duration));
        }
        else
        {
            MightDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator MightDuration(int stacks, float duration)
    {
        MightStacks += stacks;
        MightStacks = Mathf.Min(MightStacks, 25);

        if (!mightInstance) InstantiateMightClientRPC();
        UpdateMightUIClientRPC(MightStacks);

        ApplyMight();

        yield return new WaitForSeconds(duration);

        MightStacks -= stacks;
        MightStacks = Mathf.Max(MightStacks, 0);

        ApplyMight();

        if (MightStacks == 0) DestroyMightClientRPC();
        UpdateMightUIClientRPC(MightStacks);
    }

    void ApplyMight()
    {
        float mightMultiplier = MightStacks * mightPercent;
        float weaknessMultiplier = deBuffs.WeaknessStacks * deBuffs.weaknessPercent;

        float multiplier = 1 + mightMultiplier - weaknessMultiplier;

        if (player != null) player.CurrentDamage.Value = Mathf.RoundToInt(player.BaseDamage.Value * multiplier);
        if (enemy != null) enemy.CurrentDamage = Mathf.RoundToInt(enemy.BaseDamage * multiplier);
    }

    [ServerRpc]
    void MightDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(MightDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateMightClientRPC()
    {
        mightInstance = Instantiate(buff_Might, buffBar.transform);
    }

    [ClientRpc]
    void UpdateMightUIClientRPC(int stacks)
    {
        if (mightInstance) mightInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyMightClientRPC()
    {
        if (mightInstance) Destroy(mightInstance);
    }

    #endregion

    #region Alacrity

    public void Alacrity(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(AlacrityDuration(stacks, duration));
        }
        else
        {
            AlacrityDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator AlacrityDuration(int stacks, float duration)
    {
        AlacrityStacks += stacks;
        AlacrityStacks = Mathf.Min(AlacrityStacks, 25);

        if (!alacrityInstance) InstantiateAlacrityClientRPC();
        UpdateAlacrityUIClientRPC(AlacrityStacks);

        ApplyAlacrity();

        yield return new WaitForSeconds(duration);

        AlacrityStacks -= stacks;
        AlacrityStacks = Mathf.Max(AlacrityStacks, 0);

        ApplyAlacrity();

        if (AlacrityStacks == 0) DestroyAlacrityClientRPC();
        UpdateAlacrityUIClientRPC(AlacrityStacks);
    }

    void ApplyAlacrity()
    {
        float alacrityMultiplier = AlacrityStacks * alacrityPercent;
        float impedeMultiplier = deBuffs.ImpedeStacks * deBuffs.impedePercent;

        float multiplier = 1 + alacrityMultiplier - impedeMultiplier;

        if (player != null) player.CurrentCDR.Value = player.BaseCDR.Value * multiplier;
        if (enemy != null) enemy.CurrentCDR = enemy.BaseCDR * multiplier;
    }

    [ServerRpc]
    void AlacrityDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(AlacrityDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateAlacrityClientRPC()
    {
        alacrityInstance = Instantiate(buff_Alacrity, buffBar.transform);
    }

    [ClientRpc]
    void UpdateAlacrityUIClientRPC(int stacks)
    {
        if (alacrityInstance) alacrityInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyAlacrityClientRPC()
    {
        if (alacrityInstance) Destroy(alacrityInstance);
    }

    #endregion

    #region Protection

    public void Protection(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(ProtectionDuration(stacks, duration));
        }
        else
        {
            ProtectionDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator ProtectionDuration(int stacks, float duration)
    {
        ProtectionStacks += stacks;
        ProtectionStacks = Mathf.Min(ProtectionStacks, 25);

        if (!protectionInstance) InstantiateProtectionClientRPC();
        UpdateProtectionUIClientRPC(ProtectionStacks);

        ApplyProtection();

        yield return new WaitForSeconds(duration);

        ProtectionStacks -= stacks;
        ProtectionStacks = Mathf.Max(ProtectionStacks, 0);

        ApplyProtection();

        if (ProtectionStacks == 0) DestroyProtectionClientRPC();
        UpdateProtectionUIClientRPC(ProtectionStacks);
    }

    void ApplyProtection()
    {
        float protectionMultiplier = ProtectionStacks * protectionPercent;
        float vulnerabilityMultiplier = deBuffs.VulnerabilityStacks * deBuffs.vulnerabilityPercent;

        float multiplier = 1 + protectionMultiplier - vulnerabilityMultiplier;

        if (player != null) player.CurrentArmor.Value = player.BaseArmor.Value * multiplier;
        if (enemy != null) enemy.CurrentArmor = enemy.BaseArmor * multiplier;
    }

    [ServerRpc]
    void ProtectionDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(ProtectionDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateProtectionClientRPC()
    {
        protectionInstance = Instantiate(buff_Protection, buffBar.transform);
    }

    [ClientRpc]
    void UpdateProtectionUIClientRPC(int stacks)
    {
        if (protectionInstance) protectionInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyProtectionClientRPC()
    {
        if (protectionInstance) Destroy(protectionInstance);
    }

    #endregion

    #region Swiftness

    public void Swiftness(int stacks, float duration)
    {
        if (IsServer)
        {
            StartCoroutine(SwiftnessDuration(stacks, duration));
        }
        else
        {
            SwiftnessDurationServerRPC(stacks, duration);
        }
    }

    IEnumerator SwiftnessDuration(int stacks, float duration)
    {
        SwiftnessStacks += stacks;
        SwiftnessStacks = Mathf.Min(SwiftnessStacks, 25);

        if (!switnessInstance) InstantiateSwiftnessClientRPC();
        UpdateSwiftnessUIClientRPC(SwiftnessStacks);

        ApplySwiftness();

        yield return new WaitForSeconds(duration);

        SwiftnessStacks -= stacks;
        SwiftnessStacks = Mathf.Max(SwiftnessStacks, 0);

        ApplySwiftness();

        if (SwiftnessStacks == 0) DestroySwiftnessClientRPC();
        UpdateSwiftnessUIClientRPC(SwiftnessStacks);
    }

    void ApplySwiftness()
    {
        float swiftnessMultiplier = SwiftnessStacks * swiftnessPercent;
        float exhaustMultiplier = deBuffs.ExhaustStacks * deBuffs.exhaustPercent;

        float multiplier = 1 + swiftnessMultiplier - exhaustMultiplier;

        if (player != null) player.CurrentAttackSpeed.Value = player.BaseAttackSpeed.Value * multiplier;
        if (enemy != null) enemy.CurrentAttackSpeed = enemy.BaseAttackSpeed * multiplier;
    }

    [ServerRpc]
    void SwiftnessDurationServerRPC(int stacks, float duration)
    {
        StartCoroutine(SwiftnessDuration(stacks, duration));
    }

    [ClientRpc]
    void InstantiateSwiftnessClientRPC()
    {
        switnessInstance = Instantiate(buff_Swiftness, buffBar.transform);
    }

    [ClientRpc]
    void UpdateSwiftnessUIClientRPC(int stacks)
    {
        if (switnessInstance) switnessInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroySwiftnessClientRPC()
    {
        if (switnessInstance) Destroy(switnessInstance);
    }

    #endregion
}
