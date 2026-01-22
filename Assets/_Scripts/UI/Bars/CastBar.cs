using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CastBar : NetworkBehaviour
{
    [SerializeField] Image castBar;
    Coroutine currentCast;

    public void StartCast(float castTime)
    {
        if (!gameObject.activeInHierarchy) return;

        if (IsServer)
        {
            CastClientRpc(castTime);
        }
        else
        {
            CastServerRpc(castTime);
        }
    }

    [ClientRpc]
    void CastClientRpc(float castTime)
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentCast != null) StopCoroutine(currentCast);
        currentCast = StartCoroutine(HandleCast(castTime));
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void CastServerRpc(float castTime)
    {
        if (!gameObject.activeInHierarchy) return;
        CastClientRpc(castTime);
    }

    IEnumerator HandleCast(float duration)
    {
        float elapsed = 0f;
        castBar.fillAmount = 0f;
        castBar.color = Color.blue;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(elapsed / duration);
            castBar.fillAmount = fill;

            yield return null;
        }

        castBar.fillAmount = 1f;
        castBar.color = Color.green;
    }

    public void StartRecovery(float recoveryTime)
    {
        if (!gameObject.activeInHierarchy) return;

        if (IsServer)
        {
            RecoveryClientRPC(recoveryTime);
        }
        else
        {
            RecoveryServerRPC(recoveryTime);
        }
    }

    [ClientRpc]
    void RecoveryClientRPC(float recoveryTime)
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentCast != null) StopCoroutine(currentCast);
        currentCast = StartCoroutine(HandleRecovery(recoveryTime));
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void RecoveryServerRPC(float recoveryTime)
    {
        if (!gameObject.activeInHierarchy) return;
        RecoveryClientRPC(recoveryTime);
    }

    IEnumerator HandleRecovery(float duration)
    {
        float elapsed = 0f;
        castBar.fillAmount = 0f;
        castBar.color = Color.yellowNice;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(elapsed / duration);
            castBar.fillAmount = fill;

            yield return null;
        }

        castBar.fillAmount = 0f;
    }

    public void StartInterrupt()
    {
        if (!gameObject.activeInHierarchy) return;

        if (IsServer)
        {
            InterruptClientRPC();
        }
        else
        {
            InterruptServerRPC();
        }
    }

    [ClientRpc]
    void InterruptClientRPC()
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentCast != null) StopCoroutine(currentCast);
        castBar.color = Color.red;
        castBar.fillAmount = 1f;
        StartCoroutine(HandleInterrupt());
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void InterruptServerRPC()
    {
        if (!gameObject.activeInHierarchy) return;
        InterruptClientRPC();
    }

    IEnumerator HandleInterrupt()
    {
        yield return new WaitForSeconds(.2f);
        castBar.fillAmount = 0;
    }

    public void ResetCastBar()
    {
        if (!gameObject.activeInHierarchy) return;

        if (IsServer)
        {
            ResetBarClientRPC();
        }
        else
        {
            ResetServerRPC();
        }
    }

    [ClientRpc]
    void ResetBarClientRPC()
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentCast != null) StopCoroutine(currentCast);
        castBar.fillAmount = 0f;
        castBar.color = Color.clear;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void ResetServerRPC()
    {
        if (!gameObject.activeInHierarchy) return;
        ResetBarClientRPC();
    }
}
