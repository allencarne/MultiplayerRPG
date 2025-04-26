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
    GameObject immovableInstance;
    GameObject hasteInstance;

    public bool IsPhasing;
    public bool IsImmune;
    public bool IsImmovable;

    float phasingTime;
    float immuneTime;
    float immovableTime;
    int hasteStacks;

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
}
