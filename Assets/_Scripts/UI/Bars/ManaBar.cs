using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : NetworkBehaviour
{
    [Header("Stats")]
    [SerializeField] PlayerStats stats;

    [Header("UI")]
    [SerializeField] Image manaBar;
    [SerializeField] Image manaBar_Back;

    [Header("Variables")]
    bool isRegenerating = false;
    float lerpSpeed = 5f;
    Coroutine regenCoroutine;
    Coroutine lerpCoroutine;

    public override void OnNetworkSpawn()
    {
        stats.net_CurrentMana.OnValueChanged+= OnManaChanged;
        stats.net_BaseMana.OnValueChanged += OnMaxManaChanged;
        UpdateManaBar(stats.TotalMana, stats.net_CurrentMana.Value);

        if (IsServer) StartManaRegenIfNeeded();
    }

    public override void OnNetworkDespawn()
    {
        stats.net_CurrentMana.OnValueChanged -= OnManaChanged;
        stats.net_BaseMana.OnValueChanged -= OnMaxManaChanged;

        if (lerpCoroutine != null) StopCoroutine(lerpCoroutine);
        StopManaRegen();
    }

    void OnEnable()
    {
        if (IsSpawned && IsServer) StartManaRegenIfNeeded();
    }

    void OnDisable()
    {
        isRegenerating = false;
        regenCoroutine = null;
    }

    public void SpendMana(float amount)
    {
        if (IsServer)
        {
            if (stats.net_CurrentMana.Value >= amount)
                stats.net_CurrentMana.Value -= amount;   // OnManaChanged starts regen
        }
        else
        {
            SpendManaServerRpc(amount);
        }
    }

    [ServerRpc]
    void SpendManaServerRpc(float amount)
    {
        if (stats.net_CurrentMana.Value >= amount)
            stats.net_CurrentMana.Value -= amount;
    }

    void OnManaChanged(float oldValue, float newValue)
    {
        UpdateManaBar(stats.TotalMana, newValue);

        // Server only: any time mana is below max, make sure regen is running
        if (!IsServer) return;
        StartManaRegenIfNeeded();
    }

    void StartManaRegenIfNeeded()
    {
        if (!IsServer) return;
        if (isRegenerating) return;
        if (stats.net_CurrentMana.Value >= stats.TotalMana) return;

        regenCoroutine = StartCoroutine(RegenerateMana());
    }

    void StopManaRegen()
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
        isRegenerating = false;
    }

    IEnumerator RegenerateMana()
    {
        isRegenerating = true;
        float regenBuffer = 0f;

        while (stats.net_CurrentMana.Value < stats.TotalMana)
        {
            yield return new WaitForSeconds(1f);

            if (stats.net_CurrentMana.Value >= stats.TotalMana) break;

            // Accumulate fractional regen, only give whole points
            regenBuffer += stats.TotalManaRegen;
            int wholeMana = Mathf.FloorToInt(regenBuffer);

            if (wholeMana > 0)
            {
                stats.GiveMana(wholeMana);
                regenBuffer -= wholeMana;
            }
        }

        isRegenerating = false;
        regenCoroutine = null;
    }

    void UpdateManaBar(float maxMana, float currentMana)
    {
        if (maxMana <= 0) return;
        manaBar.fillAmount = currentMana / maxMana;

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }

        lerpCoroutine = StartCoroutine(LerpManaBar(currentMana / maxMana));
    }

    IEnumerator LerpManaBar(float targetFillAmount)
    {
        float currentFillAmount = manaBar_Back.fillAmount;

        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
            manaBar_Back.fillAmount = currentFillAmount;
            yield return null;
        }
    }

    void OnMaxManaChanged(float oldValue, float newValue)
    {
        UpdateManaBar(newValue, stats.net_CurrentMana.Value);
    }
}
