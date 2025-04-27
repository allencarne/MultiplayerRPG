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
}