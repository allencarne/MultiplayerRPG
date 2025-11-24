using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CastBar : NetworkBehaviour
{
    public NetworkVariable<float> FillAmount = new NetworkVariable<float>(
    0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<Color> BarColor = new NetworkVariable<Color>(
        Color.clear, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Image castBarFill;
    private Coroutine currentCastCoroutine;

    public void StartCast(float castTime)
    {
        if (currentCastCoroutine != null)
        {
            StopCoroutine(currentCastCoroutine);
        }

        currentCastCoroutine = StartCoroutine(HandleCast(castTime));
    }

    [ServerRpc]
    public void StartCastServerRpc(float castTime)
    {
        StartCast(castTime);
    }

    private IEnumerator HandleCast(float duration)
    {
        float elapsed = 0f;
        if (IsServer)
        {
            FillAmount.Value = 0f;
            BarColor.Value = Color.blue;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(elapsed / duration);

            if (IsServer)
            {
                FillAmount.Value = fill;
            }

            yield return null;
        }

        if (IsServer)
        {
            FillAmount.Value = 1f;
            BarColor.Value = Color.green;
        }
    }

    public void StartRecovery(float recoveryTime)
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentCastCoroutine != null)
        {
            StopCoroutine(currentCastCoroutine);
        }

        currentCastCoroutine = StartCoroutine(HandleRecovery(recoveryTime));
    }

    IEnumerator HandleRecovery(float duration)
    {
        float elapsed = 0f;
        if (IsServer)
        {
            FillAmount.Value = 0f;
            BarColor.Value = Color.yellowNice;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float fill = Mathf.Clamp01(elapsed / duration);

            if (IsServer)
            {
                FillAmount.Value = fill;
            }

            yield return null;
        }

        if (IsServer)
        {
            FillAmount.Value = 0f;
        }
    }

    [ServerRpc]
    public void StartRecoveryServerRpc(float recoveryTime)
    {
        StartRecovery(recoveryTime);
    }

    public void InterruptCastBar()
    {
        if (!gameObject.activeInHierarchy) return;

        if (currentCastCoroutine != null)
        {
            StopCoroutine(currentCastCoroutine);
        }

        if (IsServer)
        {
            BarColor.Value = Color.red;
            FillAmount.Value = 1f;
        }

        StartCoroutine(ResetCastBar());
    }

    [ServerRpc]
    public void InterruptServerRpc()
    {
        InterruptCastBar();
    }

    IEnumerator ResetCastBar()
    {
        yield return new WaitForSeconds(.2f);

        if (IsServer)
        {
            FillAmount.Value = 0;
        }
    }

    public void ForceReset()
    {
        if (currentCastCoroutine != null)
        {
            StopCoroutine(currentCastCoroutine);
        }

        if (IsServer)
        {
            FillAmount.Value = 0f;
            BarColor.Value = Color.clear;
        }
    }

    private void OnEnable()
    {
        FillAmount.OnValueChanged += OnFillChanged;
        BarColor.OnValueChanged += OnColorChanged;
    }

    private void OnDisable()
    {
        FillAmount.OnValueChanged -= OnFillChanged;
        BarColor.OnValueChanged -= OnColorChanged;

        // Stop any running coroutines when disabled
        if (currentCastCoroutine != null)
        {
            StopCoroutine(currentCastCoroutine);
            currentCastCoroutine = null;
        }

        // Reset state
        if (IsServer)
        {
            FillAmount.Value = 0f;
            BarColor.Value = Color.clear;
        }
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
