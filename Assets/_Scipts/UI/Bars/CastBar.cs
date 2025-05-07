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

    public void StartCast(float castTime, float attackSpeed)
    {
        float modifiedCastTime = castTime / attackSpeed;

        // Stop any previous cast in progress
        if (currentCastCoroutine != null)
        {
            StopCoroutine(currentCastCoroutine);
        }

        currentCastCoroutine = StartCoroutine(HandleCast(modifiedCastTime));
    }

    [ServerRpc]
    public void StartCastServerRpc(float castTime, float attackSpeed)
    {
        StartCast(castTime, attackSpeed);
    }

    private IEnumerator HandleCast(float duration)
    {
        float elapsed = 0f;
        if (IsServer)
        {
            FillAmount.Value = 0f;
            BarColor.Value = Color.black;
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

    public void StartRecovery(float recoveryTime, float attackSpeed)
    {
        float modifiedRecoveryTime = recoveryTime / attackSpeed;

        if (currentCastCoroutine != null)
        {
            StopCoroutine(currentCastCoroutine);
        }

        currentCastCoroutine = StartCoroutine(HandleRecovery(modifiedRecoveryTime));
    }

    IEnumerator HandleRecovery(float duration)
    {
        float elapsed = 0f;
        if (IsServer)
        {
            FillAmount.Value = 0f;
            BarColor.Value = Color.magenta;
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
    public void StartRecoveryServerRpc(float recoveryTime, float attackSpeed)
    {
        StartRecovery(recoveryTime, attackSpeed);
    }

    public void InterruptCastBar()
    {
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

    private void OnEnable()
    {
        FillAmount.OnValueChanged += OnFillChanged;
        BarColor.OnValueChanged += OnColorChanged;
    }

    private void OnDisable()
    {
        FillAmount.OnValueChanged -= OnFillChanged;
        BarColor.OnValueChanged -= OnColorChanged;
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
