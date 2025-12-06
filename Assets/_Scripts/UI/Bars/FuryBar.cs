using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FuryBar : NetworkBehaviour
{
    [SerializeField] PlayerStats stats;
    [SerializeField] Image furyBar;
    [SerializeField] Image furyBar_Back;

    private float lerpSpeed = 5f;
    private Coroutine lerpCoroutine;

    public override void OnNetworkSpawn()
    {
        stats.Fury.OnValueChanged += OnFuryChanged;
        stats.MaxFury.OnValueChanged += OnMaxFuryChanged;

        UpdateFuryBar(stats.MaxFury.Value, stats.Fury.Value);
    }

    public void UpdateFuryBar(float maxFury, float currentFury)
    {
        if (maxFury <= 0) return;

        furyBar_Back.fillAmount = currentFury / maxFury;

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(LerpHealthBarBack(currentFury / maxFury));
    }

    IEnumerator LerpHealthBarBack(float targetFillAmount)
    {
        float currentFillAmount = furyBar.fillAmount;

        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
            furyBar.fillAmount = currentFillAmount;
            yield return null;
        }
    }

    void OnFuryChanged(float oldValue, float newValue)
    {
        UpdateFuryBar(stats.MaxFury.Value, newValue);
    }

    void OnMaxFuryChanged(float oldValue, float newValue)
    {
        UpdateFuryBar(newValue, stats.Fury.Value);
    }
}
