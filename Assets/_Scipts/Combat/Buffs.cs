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

    GameObject phasingInstance;
    GameObject immuneInstance;
    GameObject hasteInstance;

    public bool IsPhasing;
    public bool IsImmune;
    public bool IsImmovable;

    int hasteStacks;
    float immuneTime;

    #region Phasing

    public void Phasing(float duration)
    {
        if (IsServer)
        {
            StartCoroutine(PhasingDuration(duration));
        }
        else
        {
            PhasingDurationServerRPC(duration);
        }
    }

    IEnumerator PhasingDuration(float duration)
    {
        PhasingClientRPC(true);

        if (!phasingInstance) InstantiatePhasingClientRPC();

        yield return new WaitForSeconds(duration);

        PhasingClientRPC(false);

        if (phasingInstance) DestroyPhasingClientRPC();
    }

    [ServerRpc]
    void PhasingDurationServerRPC(float duration)
    {
        StartCoroutine(PhasingDuration(duration));
    }

    [ClientRpc]
    void PhasingClientRPC(bool isphasing)
    {
        IsPhasing = isphasing;
        Physics2D.IgnoreLayerCollision(6, 7, isphasing); // Players ignore enemies
        Physics2D.IgnoreLayerCollision(6, 6, isphasing); // Players ignore players
        Physics2D.IgnoreLayerCollision(7, 7, isphasing); // Enemies ignore enemies
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
        //if (IsImmune) immuneTime += duration;

        if (IsServer)
        {
            StartCoroutine(ImmunityDuration(duration));
        }
        else
        {
            ImmunityDurationServerRPC(duration);
        }
    }

    IEnumerator ImmunityDuration(float duration)
    {
        ImmuneClientRPC(true);

        if (!immuneInstance) InstantiateImmuneClientRPC();

        yield return new WaitForSeconds(duration);

        if (immuneInstance) DestroyImmuneClientRPC();

        ImmuneClientRPC(false);
    }

    [ServerRpc]
    void ImmunityDurationServerRPC(float duration)
    {
        StartCoroutine(ImmunityDuration(duration));
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
        StartCoroutine(ImmovableDuration(duration));
    }

    IEnumerator ImmovableDuration(float duration)
    {
        GameObject buff = Instantiate(buff_Immovable, buffBar.transform);

        IsImmovable = true;

        yield return new WaitForSeconds(duration);

        Destroy(buff);

        IsImmovable = false;
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

        if (!hasteInstance)
        {
            InstantiateHasteClientRPC();
        }

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

        if (hasteStacks == 0)
        {
            DestroyHasteClientRPC();
        }

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
}
