using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Buff_Phase : NetworkBehaviour
{
    [Header("Variables")]
    float remainingTime = 0f;
    int activeBuffs = 0;
    bool isFixedBuffActive = false;
    public bool IsPhased;

    [Header("Components")]
    [SerializeField] GameObject UI_Bar;
    [SerializeField] GameObject UI_Prefab;
    GameObject UI_Instance;

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
    }

    public void StartPhase(float duration, bool isActive = false)
    {
        if (!IsOwner) return;

        if (duration < 0)
        {
            StartPhaseFixed(isActive);
            return;
        }

        if (duration > remainingTime)
        {
            if (IsServer)
            {
                StartUIClientRPC(duration);
            }
            else
            {
                StartUIServerRPC(duration);
            }
        }

        StartCoroutine(Duration(duration));
    }

    IEnumerator Duration(float duration)
    {
        activeBuffs++;
        IsPhased = true;

        //Physics2D.IgnoreLayerCollision(6,7,true); // Player Vs Enemy
        //Physics2D.IgnoreLayerCollision(6, 10, true); // Player Vs NPC
        //Physics2D.IgnoreLayerCollision(7, 10, true); // Enemy Vs NPC

        yield return new WaitForSeconds(duration);

        activeBuffs--;

        if (activeBuffs == 0 && isFixedBuffActive == false)
        {
            IsPhased = false;
            Debug.Log(IsPhased + "Duration");

            //Physics2D.IgnoreLayerCollision(6, 7, false); // Player Vs Enemy
            //Physics2D.IgnoreLayerCollision(6, 10, false); // Player Vs NPC
            //Physics2D.IgnoreLayerCollision(7, 10, false); // Enemy Vs NPC

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

    [ClientRpc]
    void StartUIClientRPC(float duration)
    {
        remainingTime = duration;

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

    public void StartPhaseFixed(bool isActive)
    {
        if (!IsOwner) return;

        if (isActive)
        {
            AddStackFixed();
        }
        else
        {
            RemoveStackFixed();
        }
    }

    void AddStackFixed()
    {
        isFixedBuffActive = true;
        IsPhased = true;

        //Physics2D.IgnoreLayerCollision(6, 7, true); // Player Vs Enemy
        //Physics2D.IgnoreLayerCollision(6, 10, true); // Player Vs NPC
        //Physics2D.IgnoreLayerCollision(7, 10, true); // Enemy Vs NPC

        if (IsServer)
        {
            StartFixedUIClientRPC();
        }
        else
        {
            StartFixedUIServerRPC();
        }
    }

    void RemoveStackFixed()
    {
        isFixedBuffActive = false;

        if (activeBuffs == 0 && isFixedBuffActive == false)
        {
            IsPhased = false;
            Debug.Log(IsPhased + "Fixed");

            //Physics2D.IgnoreLayerCollision(6, 7, false); // Player Vs Enemy
            //Physics2D.IgnoreLayerCollision(6, 10, false); // Player Vs NPC
            //Physics2D.IgnoreLayerCollision(7, 10, false); // Enemy Vs NPC

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

    public void PurgePhase()
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
}
