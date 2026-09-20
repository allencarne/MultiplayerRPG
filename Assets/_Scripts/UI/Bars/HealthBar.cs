using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    [Header("Combat Border")]
    [SerializeField] Image borderImage;
    [SerializeField] Color outOfCombatColor;
    [SerializeField] Color inCombatColor;

    [SerializeField] CharacterStats stats;
    [SerializeField] CharacterCombat combat;
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBar_Back;

    float lerpSpeed = 5f;
    Coroutine lerpCoroutine;

    // Regeneration
    bool isRegenerating = false;
    Coroutine regenCoroutine;

    public override void OnNetworkSpawn()
    {
        stats.net_CurrentHP.OnValueChanged += OnHealthChanged;
        stats.net_TotalHP.OnValueChanged += OnHealthChanged;
        UpdateHealthBar();

        if (combat != null)
        {
            combat.OnCombatStateChanged.AddListener(HandleCombatStateChanged);
            UpdateBorderColor(combat.InCombat.Value);
        }
        else
        {
            UpdateBorderColor(false);
        }

        // If server and out of combat and missing health, start regen
        if (IsServer && (combat == null || !combat.InCombat.Value) && stats.net_CurrentHP.Value < stats.net_TotalHP.Value)
        {
            StartHealthRegenIfNeeded();
        }
    }

    public override void OnNetworkDespawn()
    {
        stats.net_CurrentHP.OnValueChanged -= OnHealthChanged;
        stats.net_TotalHP.OnValueChanged -= OnHealthChanged;

        if (combat != null)
        {
            combat.OnCombatStateChanged.RemoveListener(HandleCombatStateChanged);
        }

        if (lerpCoroutine != null) StopCoroutine(lerpCoroutine);
        if (regenCoroutine != null) StopCoroutine(regenCoroutine);
        isRegenerating = false;
    }

    void OnHealthChanged(float oldValue, float newValue)
    {
        UpdateHealthBar();

        // Server-only regen control: if healed to full stop regen, if damaged while out of combat start combat will stop regen via combat event.
        if (!IsServer) return;

        if (newValue >= stats.net_TotalHP.Value)
        {
            StopHealthRegen();
        }
        else
        {
            // If still missing health and out of combat, ensure regen is running.
            if (combat == null || !combat.InCombat.Value)
            {
                StartHealthRegenIfNeeded();
            }
        }
    }

    void HandleCombatStateChanged(bool inCombat)
    {
        // Visuals run on every client
        UpdateBorderColor(inCombat);

        // Only server modifies health
        if (!IsServer) return;

        if (inCombat)
        {
            StopHealthRegen();
        }
        else
        {
            StartHealthRegenIfNeeded();
        }
    }

    void StartHealthRegenIfNeeded()
    {
        if (!IsServer) return;
        if (isRegenerating) return;
        if (stats.net_CurrentHP.Value >= stats.net_TotalHP.Value) return;

        regenCoroutine = StartCoroutine(RegenerateHealth());
    }

    void StopHealthRegen()
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
        isRegenerating = false;
    }

    IEnumerator RegenerateHealth()
    {
        isRegenerating = true;
        float regenBuffer = 0f;

        while (stats.net_CurrentHP.Value < stats.net_TotalHP.Value)
        {
            if (stats.isDead) break;
            if (combat != null && combat.InCombat.Value) break;

            yield return new WaitForSeconds(1f);

            if (stats.isDead) break;
            if (combat != null && combat.InCombat.Value) break;
            if (stats.net_CurrentHP.Value >= stats.net_TotalHP.Value) break;

            // Accumulate fractional regen, only heal whole points
            regenBuffer += stats.TotalHealthRegen;
            int wholeHeal = Mathf.FloorToInt(regenBuffer);

            if (wholeHeal > 0)
            {
                stats.GiveHeal(wholeHeal, HealType.Flat);
                regenBuffer -= wholeHeal;
            }
        }

        isRegenerating = false;
        regenCoroutine = null;
    }

    public void UpdateHealthBar()
    {
        float maxHealth = stats.net_TotalHP.Value;
        float currentHealth = stats.net_CurrentHP.Value;

        if (maxHealth <= 0) return;

        float fill = currentHealth / maxHealth;
        healthBar.fillAmount = fill;

        if (!gameObject.activeInHierarchy) return;

        if (lerpCoroutine != null) StopCoroutine(lerpCoroutine);
        lerpCoroutine = StartCoroutine(LerpHealthBarBack(fill));
    }

    IEnumerator LerpHealthBarBack(float targetFillAmount)
    {
        float currentFillAmount = healthBar_Back.fillAmount;

        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
            healthBar_Back.fillAmount = currentFillAmount;
            yield return null;
        }
    }

    void UpdateBorderColor(bool inCombat)
    {
        if (borderImage == null) return;
        borderImage.color = inCombat ? inCombatColor : outOfCombatColor;
    }
}
