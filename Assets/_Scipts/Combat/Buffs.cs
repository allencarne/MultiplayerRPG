using System.Collections;
using System.Collections.Generic;
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
    //public bool IsHasted;

    private float phasingElapsedTime = 0f;
    private float phasingTotalDuration = 0f;
    private float localPhasingElapsed = 0f;
    private float localPhasingTotal = 0f;

    private float immuneElapsedTime = 0f;
    private float immuneTotalDuration = 0f;
    private float localImmuneElapsed = 0f;
    private float localImmuneTotal = 0f;

    private float immovableElapsedTime = 0f;
    private float immovableTotalDuration = 0f;
    private float localImmovableElapsed = 0f;
    private float localImmovableTotal = 0f;

    private float hasteElapsed = 0f;
    private float hasteTotal = 0f;

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

    // Fields
    int durationHasteStacks = 0;
    int conditionalHasteStacks = 0;
    public int TotalHasteStacks => durationHasteStacks + conditionalHasteStacks;

    bool IsHasted => TotalHasteStacks > 0;

    GameObject durationHasteUI = null;
    GameObject conditionalHasteUI = null;

    float durationElapsed = 0f;
    float durationTotal = 0f;

    private void Update()
    {
        UpdatePhasingTime();
        UpdatePhasingUI();

        UpdateImmuneTime();
        UpdateImmuneUI();

        UpdateImmoveableTime();
        UpdateImmovableUI();

        UpdateHasteUI();
    }

    #region Phasing

    public void Phasing(float duration)
    {
        if (IsServer)
        {
            StartPhasing(duration);
        }
        else
        {
            PhasingServerRPC(duration);
        }
    }

    void StartPhasing(float duration)
    {
        phasingTotalDuration += duration;

        if (!IsPhasing)
        {
            IsPhasing = true;
        }

        float remainingTime = phasingTotalDuration - phasingElapsedTime;
        PhasingClientRPC(true, remainingTime);
    }

    [ServerRpc]
    private void PhasingServerRPC(float duration)
    {
        StartPhasing(duration);
    }

    [ClientRpc]
    private void PhasingClientRPC(bool isPhasing, float remainingTime = 0f)
    {
        IsPhasing = isPhasing;
        Physics2D.IgnoreLayerCollision(6, 7, isPhasing); // Players ignore enemies
        Physics2D.IgnoreLayerCollision(6, 6, isPhasing); // Players ignore players
        Physics2D.IgnoreLayerCollision(7, 7, isPhasing); // Enemies ignore enemies

        if (isPhasing)
        {
            if (phasingInstance == null)
            {
                phasingInstance = Instantiate(buff_Phasing, buffBar.transform);
            }

            localPhasingElapsed = 0f;
            localPhasingTotal = remainingTime;
        }
        else
        {
            if (phasingInstance != null)
            {
                Destroy(phasingInstance);
            }

            localPhasingElapsed = 0f;
            localPhasingTotal = 0f;
        }
    }

    void UpdatePhasingTime()
    {
        if (IsServer && IsPhasing)
        {
            phasingElapsedTime += Time.deltaTime;

            if (phasingElapsedTime >= phasingTotalDuration)
            {
                PhasingClientRPC(false);
                IsPhasing = false;

                phasingElapsedTime = 0f;
                phasingTotalDuration = 0f;
            }
        }
    }
    
    void UpdatePhasingUI()
    {
        if (IsPhasing && localPhasingTotal > 0f)
        {
            localPhasingElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localPhasingElapsed / localPhasingTotal);

            if (phasingInstance != null)
            {
                var ui = phasingInstance.GetComponent<StatusEffects>();
                if (ui != null)
                {
                    ui.UpdateFill(fill);
                }
            }
        }
    }

    #endregion

    #region Immune

    public void Immunity(float duration)
    {
        if (IsServer)
        {
            StartImmune(duration);
        }
        else
        {
            ImmuneServerRPC(duration);
        }
    }

    void StartImmune(float duration)
    {
        immuneTotalDuration += duration;

        if (!IsImmune)
        {
            IsImmune = true;
        }

        float remainingTime = immuneTotalDuration - immuneElapsedTime;
        ImmuneClientRPC(true, remainingTime);
    }

    [ServerRpc]
    private void ImmuneServerRPC(float duration)
    {
        StartImmune(duration);
    }

    [ClientRpc]
    private void ImmuneClientRPC(bool isImmune, float remainingTime = 0f)
    {
        IsImmune = isImmune;

        if (isImmune)
        {
            if (immuneInstance == null)
            {
                immuneInstance = Instantiate(buff_Immune, buffBar.transform);
            }

            localImmuneElapsed = 0f;
            localImmuneTotal = remainingTime;
        }
        else
        {
            if (immuneInstance != null)
            {
                Destroy(immuneInstance);
            }

            localImmuneElapsed = 0f;
            localImmuneTotal = 0f;
        }
    }

    void UpdateImmuneTime()
    {
        if (IsServer && IsImmune)
        {
            immuneElapsedTime += Time.deltaTime;

            if (immuneElapsedTime >= immuneTotalDuration)
            {
                ImmuneClientRPC(false);
                IsImmune = false;
                immuneElapsedTime = 0f;
                immuneTotalDuration = 0f;
            }
        }
    }

    void UpdateImmuneUI()
    {
        if (IsImmune && localImmuneTotal > 0f)
        {
            localImmuneElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localImmuneElapsed / localImmuneTotal);

            if (immuneInstance != null)
            {
                var ui = immuneInstance.GetComponent<StatusEffects>();
                if (ui != null)
                    ui.UpdateFill(fill);
            }
        }
    }

    #endregion

    #region Immovable

    public void Immovable(float duration)
    {
        if (IsServer)
        {
            StartImmovable(duration);
        }
        else
        {
            ImmovableServerRPC(duration);
        }
    }

    void StartImmovable(float duration)
    {
        immovableTotalDuration += duration;

        if (!IsImmovable)
        {
            IsImmovable = true;
        }

        float remainingTime = immovableTotalDuration - immovableElapsedTime;
        ImmovableClientRPC(true, remainingTime);
    }

    [ServerRpc]
    private void ImmovableServerRPC(float duration)
    {
        StartImmovable(duration);
    }

    [ClientRpc]
    private void ImmovableClientRPC(bool isImmovable, float remainingTime = 0f)
    {
        IsImmovable = isImmovable;

        if (isImmovable)
        {
            if (immovableInstance == null)
            {
                immovableInstance = Instantiate(buff_Immovable, buffBar.transform);
            }

            localImmovableElapsed = 0f;
            localImmovableTotal = remainingTime;
        }
        else
        {
            if (immovableInstance != null)
            {
                Destroy(immovableInstance);
            }

            localImmovableElapsed = 0f;
            localImmovableTotal = 0f;
        }
    }

    void UpdateImmoveableTime()
    {
        if (IsServer && IsImmovable)
        {
            immovableElapsedTime += Time.deltaTime;

            if (immovableElapsedTime >= immovableTotalDuration)
            {
                ImmovableClientRPC(false);
                IsImmovable = false;
                immovableElapsedTime = 0f;
                immovableTotalDuration = 0f;
            }
        }
    }

    void UpdateImmovableUI()
    {
        if (IsImmovable && localImmovableTotal > 0f)
        {
            localImmovableElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(localImmovableElapsed / localImmovableTotal);

            if (immovableInstance != null)
            {
                var ui = immovableInstance.GetComponent<StatusEffects>();
                if (ui != null)
                    ui.UpdateFill(fill);
            }
        }
    }

    #endregion

    #region Haste

    public void Haste(int stacks, float? duration = null)
    {
        if (!IsOwner) return;

        if (IsServer)
        {
            if (duration.HasValue)
                StartCoroutine(StartDurationHaste(stacks, duration.Value));
        }
        else
        {
            if (duration.HasValue)
                HasteServerRPC(stacks, duration.Value);
        }
    }

    [ServerRpc]
    void HasteServerRPC(int stacks, float duration)
    {
        StartCoroutine(StartDurationHaste(stacks, duration));
    }

    IEnumerator StartDurationHaste(int stacks, float duration)
    {
        durationHasteStacks += stacks;
        durationHasteStacks = Mathf.Min(durationHasteStacks, 25);

        UpdateHasteUI();
        RecalculateSpeed();

        if (duration > durationTotal - durationElapsed)
        {
            HasteUIDurationClientRPC(durationHasteStacks, duration);
        }

        yield return new WaitForSeconds(duration);

        durationHasteStacks -= stacks;
        durationHasteStacks = Mathf.Max(durationHasteStacks, 0);

        UpdateHasteUI();
        RecalculateSpeed();

        HasteUIDurationClientRPC(durationHasteStacks);
    }

    public void SetConditionalHaste(int stacks)
    {
        conditionalHasteStacks += stacks;
        conditionalHasteStacks = Mathf.Clamp(conditionalHasteStacks, 0, 25);

        UpdateHasteUI();
        RecalculateSpeed();
        HasteUIConditionalClientRPC(conditionalHasteStacks);
    }

    public void SetExactConditionalHaste(int targetStacks)
    {
        targetStacks = Mathf.Clamp(targetStacks, 0, 25);
        if (conditionalHasteStacks == targetStacks) return;

        conditionalHasteStacks = targetStacks;

        UpdateHasteUI();
        RecalculateSpeed();
        HasteUIConditionalClientRPC(conditionalHasteStacks);
    }

    public void RemoveConditionalHaste(int stacks)
    {
        conditionalHasteStacks -= stacks;
        conditionalHasteStacks = Mathf.Clamp(conditionalHasteStacks, 0, 25);

        UpdateHasteUI();
        RecalculateSpeed();
        HasteUIConditionalClientRPC(conditionalHasteStacks);
    }

    void RecalculateSpeed()
    {
        float hasteMultiplier = TotalHasteStacks * hastePercent;
        float slowMultiplier = deBuffs.SlowStacks * deBuffs.slowPercent;
        float multiplier = 1 + hasteMultiplier - slowMultiplier;

        if (player != null) player.CurrentSpeed.Value = player.BaseSpeed.Value * multiplier;
        if (enemy != null) enemy.CurrentSpeed = enemy.BaseSpeed * multiplier;
    }

    [ClientRpc]
    void HasteUIDurationClientRPC(int stacks, float remaining = -1f)
    {
        if (stacks > 0)
        {
            if (durationHasteUI == null)
                durationHasteUI = Instantiate(buff_Haste, buffBar.transform);

            durationHasteUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();

            if (remaining > 0f)
            {
                durationElapsed = 0f;
                durationTotal = remaining;
            }
        }
        else
        {
            if (durationHasteUI != null)
            {
                Destroy(durationHasteUI);
                durationHasteUI = null;
            }

            durationElapsed = 0f;
            durationTotal = 0f;
        }
    }

    [ClientRpc]
    void HasteUIConditionalClientRPC(int stacks)
    {
        if (stacks > 0)
        {
            if (conditionalHasteUI == null)
                conditionalHasteUI = Instantiate(buff_Haste, buffBar.transform);

            conditionalHasteUI.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
        else
        {
            if (conditionalHasteUI != null)
            {
                Destroy(conditionalHasteUI);
                conditionalHasteUI = null;
            }
        }
    }

    [ClientRpc]
    void HasteSpeedClientRPC(int stacks)
    {
        if (!IsOwner) return;
        HasteSpeedServerRPC(stacks);
    }

    [ServerRpc]
    void HasteSpeedServerRPC(int stacks)
    {
        HasteStacks = stacks;

        float hasteMultiplier = HasteStacks * hastePercent;
        float slowMultiplier = deBuffs.SlowStacks * deBuffs.slowPercent;
        float multiplier = 1 + hasteMultiplier - slowMultiplier;

        if (player != null) player.CurrentSpeed.Value = player.BaseSpeed.Value * multiplier;
        if (enemy != null) enemy.CurrentSpeed = enemy.BaseSpeed * multiplier;
    }

    void UpdateHasteUI()
    {
        if (durationTotal > 0f && durationHasteUI != null)
        {
            durationElapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(durationElapsed / durationTotal);

            var ui = durationHasteUI.GetComponent<StatusEffects>();
            if (ui != null) ui.UpdateFill(fill);
        }
    }

        #endregion

        /*

        #region Might

        public void Might(int stacks, float duration)
        {
            if (!IsOwner) return;

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
            if (!IsOwner) return;

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
            if (!IsOwner) return;

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
            if (!IsOwner) return;

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

        */
    }
