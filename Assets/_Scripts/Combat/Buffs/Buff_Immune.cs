using Unity.Netcode;
using UnityEngine;

public class Buff_Immune : NetworkBehaviour
{
    [Header("Variables")]
    float remainingTime = 0f;
    int activeBuffs = 0;
    bool isFixedBuffActive = false;
    public bool IsImmune;

    [Header("Components")]
    [SerializeField] GameObject UI_Bar;
    [SerializeField] GameObject UI_Prefab;
    GameObject UI_Instance;

    void Update()
    {
        if (activeBuffs == 0) return;

        if (Time.time >= remainingTime)
        {
            RemoveStack(false);
        }
    }

    public void StartImmune(float duration, bool isActive = false)
    {
        if (!IsOwner) return;

        if (duration < 0)
        {
            StartImmuneFixed(isActive);
            return;
        }

        remainingTime = Time.time + duration;

        if (IsServer)
        {
            StartUIClientRPC(duration);
        }
        else
        {
            StartUIServerRPC(duration);
        }

        AddStack(false);
    }

    void AddStack(bool isFixed)
    {
        IsImmune = true;

        if (isFixed)
        {
            isFixedBuffActive = true;
        }
        else
        {
            activeBuffs++;
        }

        if (IsServer)
        {
            if (isFixed) StartFixedUIClientRPC();
        }
        else
        {
            if (isFixed) StartFixedUIServerRPC();
        }
    }

    void RemoveStack(bool isFixed)
    {
        if (isFixed)
        {
            isFixedBuffActive = false;
        }
        else
        {
            activeBuffs--;
        }

        if (activeBuffs == 0 && isFixedBuffActive == false)
        {
            IsImmune = false;

            if (IsServer)
            {
                DestroyUIClientRPC();
            }
            else
            {
                DestroyUIServerRPC();
            }
        }
    }

    public void StartImmuneFixed(bool isActive)
    {
        if (!IsOwner) return;

        if (isActive)
        {
            AddStack(true);
        }
        else
        {
            RemoveStack(true);
        }
    }

    public void PurgeImmune()
    {
        StopAllCoroutines();

        if (IsServer)
        {
            DestroyUIClientRPC();
        }
        else
        {
            DestroyUIServerRPC();
        }
    }

    [ClientRpc]
    void StartUIClientRPC(float duration)
    {
        if (UI_Instance == null)
        {
            UI_Instance = Instantiate(UI_Prefab, UI_Bar.transform);
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
    void DestroyUIClientRPC()
    {
        if (UI_Instance != null) Destroy(UI_Instance);
    }

    [ServerRpc]
    void DestroyUIServerRPC()
    {
        DestroyUIClientRPC();
    }

    [ClientRpc]
    void StartFixedUIClientRPC()
    {
        if (UI_Instance == null)
        {
            UI_Instance = Instantiate(UI_Prefab, UI_Bar.transform);
        }
    }

    [ServerRpc]
    void StartFixedUIServerRPC()
    {
        StartFixedUIClientRPC();
    }
}
