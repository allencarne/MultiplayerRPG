using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buffs : NetworkBehaviour
{
    [SerializeField] GameObject buffBar;
    public Buff_Phase phase;
    public Buff_Immune immune;
    public Buff_Immoveable immoveable;
    public Buff_Haste haste;
    public Buff_Might might;
    public Buff_Alacrity alacrity;
    public Buff_Protection protection;
    public Buff_Swiftness swiftness;

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
