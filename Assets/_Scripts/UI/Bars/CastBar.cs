using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CastBar : NetworkBehaviour
{
    public NetworkVariable<float> FillAmount = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Color> BarColor = new NetworkVariable<Color>(Color.clear, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [SerializeField] Image castBarFill;
    Coroutine currentCastCoroutine;

    public override void OnNetworkSpawn()
    {
        FillAmount.OnValueChanged += OnFillChanged;
        BarColor.OnValueChanged += OnColorChanged;
    }

    public override void OnNetworkDespawn()
    {
        FillAmount.OnValueChanged -= OnFillChanged;
        BarColor.OnValueChanged -= OnColorChanged;
    }

    public void StartCast(float castTime)
    {
        if (IsServer)
        {
            Cast(castTime);
        }
        else
        {
            CastServerRpc(castTime);
        }
    }

    void Cast(float castTime)
    {
        if (currentCastCoroutine != null) StopCoroutine(currentCastCoroutine);
        currentCastCoroutine = StartCoroutine(HandleCast(castTime));
    }

    [ServerRpc(RequireOwnership = false)]
    public void CastServerRpc(float castTime)
    {
        Cast(castTime);
    }

    IEnumerator HandleCast(float duration)
    {
        float elapsed = 0f;
        FillAmount.Value = 0f;
        BarColor.Value = Color.blue;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(elapsed / duration);
            FillAmount.Value = fill;

            yield return null;
        }

        FillAmount.Value = 1f;
        BarColor.Value = Color.green;
    }

    public void StartRecovery(float recoveryTime)
    {
        if (!gameObject.activeInHierarchy) return;

        if (IsServer)
        {
            Recovery(recoveryTime);
        }
        else
        {
            RecoveryServerRPC(recoveryTime);
        }
    }

    void Recovery(float recoveryTime)
    {
        if (currentCastCoroutine != null) StopCoroutine(currentCastCoroutine);
        currentCastCoroutine = StartCoroutine(HandleRecovery(recoveryTime));
    }

    [ServerRpc(RequireOwnership = false)]
    void RecoveryServerRPC(float recoveryTime)
    {
        Recovery(recoveryTime);
    }

    IEnumerator HandleRecovery(float duration)
    {
        float elapsed = 0f;
        FillAmount.Value = 0f;
        BarColor.Value = Color.yellowNice;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(elapsed / duration);
            FillAmount.Value = fill;

            yield return null;
        }

        FillAmount.Value = 0f;
    }

    public void InterruptCastBar()
    {
        if (!gameObject.activeInHierarchy) return;

        if (IsServer)
        {
            Interrupt();
        }
        else
        {
            InterruptServerRPC();
        }
    }

    void Interrupt()
    {
        if (currentCastCoroutine != null) StopCoroutine(currentCastCoroutine);
        BarColor.Value = Color.red;
        FillAmount.Value = 1f;
        StartCoroutine(HandleInterrupt());
    }

    [ServerRpc(RequireOwnership = false)]
    void InterruptServerRPC()
    {
        Interrupt();
    }

    IEnumerator HandleInterrupt()
    {
        yield return new WaitForSeconds(.2f);

        FillAmount.Value = 0;
    }

    public void ResetCastBar()
    {
        if (IsServer)
        {
            ResetBar();
        }
        else
        {
            ResetServerRPC();
        }
    }

    void ResetBar()
    {
        if (currentCastCoroutine != null) StopCoroutine(currentCastCoroutine);
        FillAmount.Value = 0f;
        BarColor.Value = Color.clear;
    }

    [ServerRpc(RequireOwnership = false)]
    void ResetServerRPC()
    {
        ResetBar();
    }

    private void OnFillChanged(float previous, float current)
    {
        castBarFill.fillAmount = current;
    }

    private void OnColorChanged(Color previous, Color current)
    {
        castBarFill.color = current;
    }
}
