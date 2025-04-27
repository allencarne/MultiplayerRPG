using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buffs : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;

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

    int hasteStacks;
    int mightStacks;
    int alacrityStacks;
    int protectionStacks;
    int swiftnessStacks;

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

        if (!phasingInstance) InstantiatePhasingClientRPC();

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
        phasingInstance = Instantiate(buff_Phasing, buffBar.transform);
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
        hasteStacks += stacks;
        hasteStacks = Mathf.Min(hasteStacks, 25);

        if (!hasteInstance) InstantiateHasteClientRPC();

        UpdateHasteUIClientRPC(hasteStacks);

        // Apply Haste
        if (player != null) player.CurrentSpeed.Value = player.BaseSpeed.Value + hasteStacks;
        if (enemy != null) enemy.CurrentSpeed = enemy.BaseSpeed + hasteStacks;

        yield return new WaitForSeconds(duration);

        hasteStacks -= stacks;
        hasteStacks = Mathf.Max(hasteStacks, 0);

        // Apply Haste
        if (player != null) player.CurrentSpeed.Value = player.BaseSpeed.Value + hasteStacks;
        if (enemy != null) enemy.CurrentSpeed = enemy.BaseSpeed + hasteStacks;

        if (hasteStacks == 0) DestroyHasteClientRPC();

        UpdateHasteUIClientRPC(hasteStacks);
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
        hasteInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyHasteClientRPC()
    {
        Destroy(hasteInstance);
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
        mightStacks += stacks;
        mightStacks = Mathf.Min(mightStacks, 25);

        if (!mightInstance) InstantiateMightClientRPC();

        UpdateMightUIClientRPC(mightStacks);

        // Apply Buff
        if (player != null) player.CurrentDamage.Value = player.BaseDamage.Value + mightStacks;
        if (enemy != null) enemy.CurrentDamage = enemy.BaseDamage + mightStacks;

        yield return new WaitForSeconds(duration);

        mightStacks -= stacks;
        mightStacks = Mathf.Max(mightStacks, 0);

        // Apply Buff
        if (player != null) player.CurrentDamage.Value = player.BaseDamage.Value + mightStacks;
        if (enemy != null) enemy.CurrentDamage = enemy.BaseDamage + mightStacks;

        if (mightStacks == 0) DestroyMightClientRPC();

        UpdateMightUIClientRPC(mightStacks);
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
        mightInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyMightClientRPC()
    {
        Destroy(mightInstance);
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
        alacrityStacks += stacks;
        alacrityStacks = Mathf.Min(alacrityStacks, 25);

        if (!alacrityInstance) InstantiateAlacrityClientRPC();

        UpdateAlacrityUIClientRPC(alacrityStacks);

        // Apply Buff
        if (player != null) player.CurrentCDR.Value = player.BaseCDR.Value + alacrityStacks;
        if (enemy != null) enemy.CurrentCDR = enemy.BaseCDR + alacrityStacks;

        yield return new WaitForSeconds(duration);

        alacrityStacks -= stacks;
        alacrityStacks = Mathf.Max(alacrityStacks, 0);

        // Apply Buff
        if (player != null) player.CurrentCDR.Value = player.BaseCDR.Value + alacrityStacks;
        if (enemy != null) enemy.CurrentCDR = enemy.BaseCDR + alacrityStacks;

        if (alacrityStacks == 0) DestroyAlacrityClientRPC();

        UpdateAlacrityUIClientRPC(alacrityStacks);
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
        alacrityInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyAlacrityClientRPC()
    {
        Destroy(alacrityInstance);
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
        protectionStacks += stacks;
        protectionStacks = Mathf.Min(protectionStacks, 25);

        if (!protectionInstance) InstantiateProtectionClientRPC();

        UpdateProtectionUIClientRPC(protectionStacks);

        // Apply Buff
        if (player != null) player.CurrentArmor.Value = player.BaseArmor.Value + protectionStacks;
        if (enemy != null) enemy.CurrentArmor = enemy.BaseArmor + protectionStacks;

        yield return new WaitForSeconds(duration);

        protectionStacks -= stacks;
        protectionStacks = Mathf.Max(protectionStacks, 0);

        // Apply Buff
        if (player != null) player.CurrentArmor.Value = player.BaseArmor.Value + protectionStacks;
        if (enemy != null) enemy.CurrentArmor = enemy.BaseArmor + protectionStacks;

        if (protectionStacks == 0) DestroyProtectionClientRPC();

        UpdateProtectionUIClientRPC(protectionStacks);
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
        protectionInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroyProtectionClientRPC()
    {
        Destroy(protectionInstance);
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
        swiftnessStacks += stacks;
        swiftnessStacks = Mathf.Min(swiftnessStacks, 25);

        if (!switnessInstance) InstantiateSwiftnessClientRPC();

        UpdateSwiftnessUIClientRPC(swiftnessStacks);

        // Apply Buff
        if (player != null) player.CurrentAttackSpeed.Value = player.BaseAttackSpeed.Value + swiftnessStacks;
        if (enemy != null) enemy.CurrentAttackSpeed = enemy.BaseAttackSpeed + swiftnessStacks;

        yield return new WaitForSeconds(duration);

        swiftnessStacks -= stacks;
        swiftnessStacks = Mathf.Max(swiftnessStacks, 0);

        // Apply Buff
        if (player != null) player.CurrentAttackSpeed.Value = player.BaseAttackSpeed.Value + swiftnessStacks;
        if (enemy != null) enemy.CurrentAttackSpeed = enemy.BaseAttackSpeed + swiftnessStacks;

        if (swiftnessStacks == 0) DestroySwiftnessClientRPC();

        UpdateSwiftnessUIClientRPC(swiftnessStacks);
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
        switnessInstance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
    }

    [ClientRpc]
    void DestroySwiftnessClientRPC()
    {
        Destroy(switnessInstance);
    }

    #endregion
}
