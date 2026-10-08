using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Buff_Regeneration : NetworkBehaviour
{
    [Header("Particle")]
    [SerializeField] Transform parentTransform;
    [SerializeField] GameObject ParticlePrefab;
    GameObject particleInstance;

    [Header("Variables")]
    int maxStacks = 9;
    int durBuff = 0;
    int fixedBuff = 0;
    int TotalStacks => durBuff + fixedBuff;

    float remainingTime = 0f;
    float nextTick = 0f;

    [Header("Components")]
    [SerializeField] CharacterStats stats;
    [SerializeField] GameObject UI_Bar;
    [SerializeField] GameObject UI_Prefab;
    GameObject UI_Instance;

    void Update()
    {
        bool atFullHealth = stats.net_CurrentHealth.Value >= stats.net_TotalHealth.Value;

        if ((durBuff > 0 || fixedBuff > 0) && atFullHealth)
        {
            if (IsOwner && fixedBuff > 0)
            {
                StartBuff(-fixedBuff, -1);
            }
        }
        else if (durBuff > 0 || fixedBuff > 0)
        {
            if (Time.time >= nextTick)
            {
                float healPerStack = stats.TotalHealthRegen;
                float healAmount = TotalStacks * healPerStack;

                if (IsServer)
                    stats.GiveHeal(healAmount, HealType.Flat);
                else
                    GiveHealServerRpc(healAmount);

                nextTick = Time.time + 1f;
            }
        }

        if (durBuff == 0) return;
        if (Time.time >= remainingTime) RemoveStack(false);
    }

    public void StartBuff(int stacks, float duration)
    {
        if (duration < 0)
        {
            StartBuffFixed(stacks);
            return;
        }

        remainingTime = Time.time + duration;
        if (TotalStacks == 0) nextTick = Time.time;

        if (IsServer)
        {
            StartUIClientRPC(duration);
        }
        else
        {
            StartUIServerRPC(duration);
        }

        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        for (int i = 0; i < stacksToAdd; i++)
        {
            AddStack(false);
        }
    }

    void AddStack(bool isFixed)
    {
        if (isFixed)
        {
            fixedBuff++;
        }
        else
        {
            durBuff++;
        }

        if (TotalStacks == 1) nextTick = Time.time;

        if (IsServer)
        {
            if (isFixed) StartFixedUIClientRPC();
            UpdateUIClientRPC(TotalStacks);
        }
        else
        {
            if (isFixed) StartFixedUIServerRPC();
            UpdateUIServerRPC(TotalStacks);
        }
    }

    void RemoveStack(bool isFixed)
    {
        if (isFixed)
        {
            fixedBuff--;
        }
        else
        {
            durBuff--;
        }

        if (IsServer)
        {
            UpdateUIClientRPC(TotalStacks);
            DestroyUIClientRPC(TotalStacks);
        }
        else
        {
            UpdateUIServerRPC(TotalStacks);
            DestroyUIServerRPC(TotalStacks);
        }
    }

    void StartBuffFixed(int stacks)
    {
        if (stacks < 0)
        {
            if (fixedBuff < 1) return;

            int stacksToRemove = Mathf.Abs(stacks);
            stacksToRemove = Mathf.Min(stacksToRemove, fixedBuff);

            for (int i = 0; i < stacksToRemove; i++)
            {
                RemoveStack(true);
            }

            return;
        }

        int stacksToAdd = Mathf.Min(stacks, maxStacks - TotalStacks);
        if (stacksToAdd <= 0) return;

        for (int i = 0; i < stacksToAdd; i++)
        {
            AddStack(true);
        }
    }

    public void PurgeBuff()
    {
        durBuff = 0;
        fixedBuff = 0;

        if (IsServer)
        {
            UpdateUIClientRPC(TotalStacks);
            DestroyUIClientRPC(TotalStacks);
        }
        else
        {
            UpdateUIServerRPC(TotalStacks);
            DestroyUIServerRPC(TotalStacks);
        }
    }

    [ClientRpc]
    void StartUIClientRPC(float duration)
    {
        if (UI_Bar == null || !UI_Bar.activeInHierarchy) return;

        if (UI_Instance == null)
        {
            UI_Instance = Instantiate(UI_Prefab, UI_Bar.transform);
        }

        if (particleInstance == null)
        {
            particleInstance = Instantiate(ParticlePrefab, parentTransform);
        }

        StatusEffects se = UI_Instance.GetComponent<StatusEffects>();
        se.StartUI(duration);
    }

    [ServerRpc]
    void StartUIServerRPC(float duration)
    {
        StartUIClientRPC(duration);
    }

    [ClientRpc]
    void UpdateUIClientRPC(float stacks)
    {
        if (UI_Instance != null)
        {
            UI_Instance.GetComponentInChildren<TextMeshProUGUI>().text = stacks.ToString();
        }
    }

    [ServerRpc]
    void UpdateUIServerRPC(float stacks)
    {
        UpdateUIClientRPC(stacks);
    }

    [ClientRpc]
    void DestroyUIClientRPC(float stacks)
    {
        if (stacks == 0)
        {
            if (UI_Instance != null) Destroy(UI_Instance);
            if (particleInstance != null) Destroy(particleInstance);
        }
    }

    [ServerRpc]
    void DestroyUIServerRPC(float stacks)
    {
        DestroyUIClientRPC(stacks);
    }

    [ClientRpc]
    void StartFixedUIClientRPC()
    {
        if (UI_Instance == null)
        {
            UI_Instance = Instantiate(UI_Prefab, UI_Bar.transform);
        }

        if (particleInstance == null)
        {
            particleInstance = Instantiate(ParticlePrefab, parentTransform);
        }
    }

    [ServerRpc]
    void StartFixedUIServerRPC()
    {
        StartFixedUIClientRPC();
    }

    [ServerRpc]
    void GiveHealServerRpc(float healAmount)
    {
        stats.GiveHeal(healAmount, HealType.Flat);
    }
}
