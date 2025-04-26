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
    float phasingTime;

    private Coroutine phasingCoroutine = null;
    private object immunCoroutine;

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

        if (phasingCoroutine == null)
        {
            phasingCoroutine = StartCoroutine(PhasingDuration());
        }
    }

    private IEnumerator PhasingDuration()
    {
        PhasingClientRPC(true);

        if (!phasingInstance)
        {
            InstantiatePhasingClientRPC();
        }

        while (phasingTime > 0f)
        {
            phasingTime -= Time.deltaTime;
            yield return null;
        }

        PhasingClientRPC(false);

        if (phasingInstance)
        {
            DestroyPhasingClientRPC();
        }

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

        if (immunCoroutine == null)
        {
            immunCoroutine = StartCoroutine(ImmuneDuration());
        }
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

        immunCoroutine = null;
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
