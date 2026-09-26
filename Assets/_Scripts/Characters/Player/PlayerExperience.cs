using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerExperience : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] PlayerStats stats;
    bool statsInitialized;

    [Header("UI")]
    [SerializeField] Image frontXpBar;
    [SerializeField] Image backXpBar;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] TextMeshProUGUI levelText;

    [Header("Events")]
    public UnityEvent<float> OnEXPGained;
    public UnityEvent OnEXP;
    public UnityEvent OnLevelUp;

    // Single coroutine reference so we can stop/restart cleanly
    Coroutine xpCoroutine;

    // Duration limits for scaling with magnitude of change
    readonly float minLerpDuration = 0.20f;
    readonly float maxLerpDuration = 1.50f;

    public override void OnNetworkSpawn()
    {
        stats.RequiredExperience.OnValueChanged += OnReqExperienceChanged;
        stats.CurrentExperience.OnValueChanged += OnExperienceChanged;
        stats.PlayerLevel.OnValueChanged += OnLevelChanged;

        // Display the player's current level when they first spawn.
        levelText.text = stats.PlayerLevel.Value.ToString();

        // Display the current experience and required experience.
        experienceText.text = stats.CurrentExperience.Value + "/" + stats.RequiredExperience.Value;

        // Prevent dividing by zero if RequiredExperience hasn't been calculated yet.
        if (stats.RequiredExperience.Value > 0)
        {
            // Calculate the percentage full the experience bar should be.
            float fill = stats.CurrentExperience.Value / stats.RequiredExperience.Value;

            // Set both bars to the correct value immediately.
            frontXpBar.fillAmount = fill;
            backXpBar.fillAmount = fill;

            // Mark that our UI has finished initializing.
            statsInitialized = true;
        }
    }

    private void OnDisable()
    {
        stats.RequiredExperience.OnValueChanged -= OnReqExperienceChanged;
        stats.CurrentExperience.OnValueChanged -= OnExperienceChanged;
        stats.PlayerLevel.OnValueChanged -= OnLevelChanged;
    }

    private void Update()
    {
        // Only allow the owner of this player object to run this code.
        if (!IsOwner) return;

        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.F3))
        {
            IncreaseEXP(1);
        }

        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.F4))
        {
            IncreaseEXP(10);
        }

        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.F5))
        {
            IncreaseEXP(50);
        }
    }

    public void Initialize()
    {
        if (IsServer)
        {
            // Calculate the amount of experience required for the current level.
            stats.RequiredExperience.Value = CalculateRequiredXp();
        }
        else
        {
            // Ask the server to calculate the amount of experience required for the current level.
            CalculateServerRPC();
        }
    }

    [ServerRpc]
    void CalculateServerRPC()
    {
        stats.RequiredExperience.Value = CalculateRequiredXp();
    }

    int CalculateRequiredXp() => stats.ScalingData.CalculateRequiredXp(stats.PlayerLevel.Value);

    IEnumerator LerpXpBar()
    {
        // Track how much time has passed.
        float elapsed = 0f;

        // Remember where the bar is starting.
        float startFill = frontXpBar.fillAmount;

        // Avoid divide by zero
        float required = Mathf.Max(1f, stats.RequiredExperience.Value);
        // Calculate where the bar should end.
        float targetFill = stats.CurrentExperience.Value / required;

        // If there's effectively no change, short-circuit.
        if (Mathf.Approximately(startFill, targetFill))
        {
            frontXpBar.fillAmount = targetFill;
            backXpBar.fillAmount = targetFill;
            yield break;
        }

        // Delta magnitude to scale duration
        float delta = Mathf.Abs(targetFill - startFill);
        float duration = Mathf.Lerp(minLerpDuration, maxLerpDuration, delta);

        // Detect level-up crossing: target is less than start -> we've wrapped to next level
        if (targetFill < startFill - 0.0001f)
        {
            // Phase 1: animate current front from startFill -> 1.0 (fill to 100%)
            backXpBar.fillAmount = 1f;
            elapsed = 0f;
            float phaseDurationA = Mathf.Lerp(minLerpDuration, maxLerpDuration, 1f - startFill);
            while (elapsed < phaseDurationA)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / phaseDurationA));
                frontXpBar.fillAmount = Mathf.Lerp(startFill, 1f, t);
                yield return null;
            }

            frontXpBar.fillAmount = 1f;

            // small frame pause to ensure visuals update
            yield return null;

            // Reset visuals for new level
            frontXpBar.fillAmount = 0f;
            backXpBar.fillAmount = targetFill;

            // Phase 2: animate 0 -> targetFill (the leftover XP after level up)
            elapsed = 0f;
            float phaseDurationB = Mathf.Lerp(minLerpDuration, maxLerpDuration, targetFill);
            while (elapsed < phaseDurationB)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / phaseDurationB));
                frontXpBar.fillAmount = Mathf.Lerp(0f, targetFill, t);
                yield return null;
            }

            frontXpBar.fillAmount = targetFill;
            yield break;
        }
        else
        {
            // Normal single-phase animation: startFill -> targetFill
            backXpBar.fillAmount = targetFill;
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                frontXpBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);
                yield return null;
            }

            frontXpBar.fillAmount = targetFill;
            yield break;
        }
    }

    void StartLerpXpBar()
    {
        if (xpCoroutine != null)
        {
            StopCoroutine(xpCoroutine);
            xpCoroutine = null;
        }

        xpCoroutine = StartCoroutine(LerpXpBar());
    }

    public void IncreaseEXP(float xpGained)
    {
        if (IsServer)
        {
            // Directly add experience.
            stats.CurrentExperience.Value += xpGained;
        }
        else
        {
            // Ask the server to add the experience.
            IncreaseEXPServerRPC(xpGained);
        }

        // Notify listeners how much experience was earned.
        OnEXPGained?.Invoke(xpGained);

        // Notify listeners that experience changed.
        OnEXP?.Invoke();
    }

    [ServerRpc]
    void IncreaseEXPServerRPC(float xpGained)
    {
        stats.CurrentExperience.Value += xpGained;
    }

    void LevelUp()
    {
        // Increase the player's level by one.
        stats.PlayerLevel.Value++;

        // Award an attribute point.
        stats.IncreaseAttribuePoints();

        // Fully heal the player (100% heal).
        stats.GiveHeal(100, HealType.Percentage);

        // Reset the experience bar visuals.
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;

        // Carry any leftover experience into the next level.
        stats.CurrentExperience.Value = Mathf.RoundToInt(stats.CurrentExperience.Value - stats.RequiredExperience.Value);

        // Calculate how much experience is needed for the next level.
        stats.RequiredExperience.Value = CalculateRequiredXp();

        // Notify any listeners that the player leveled up.
        OnLevelUp?.Invoke();
    }

    void OnReqExperienceChanged(float oldValue, float newValue)
    {
        // Refresh the displayed level.
        levelText.text = stats.PlayerLevel.Value.ToString();

        // Refresh the displayed experience numbers.
        experienceText.text = stats.CurrentExperience.Value + "/" + stats.RequiredExperience.Value;

        // Start animating the experience bar (stops any existing animation).
        StartLerpXpBar();

        // Mark that initialization has completed.
        statsInitialized = true;
    }

    void OnExperienceChanged(float oldValue, float newValue)
    {
        // Ignore experience changes until the UI has finished initializing.
        if (!statsInitialized) return;

        // Update the displayed experience text.
        experienceText.text = stats.CurrentExperience.Value + "/" + stats.RequiredExperience.Value;

        // Start animating the experience bar (stops any existing animation).
        StartLerpXpBar();

        // Check if we've reached enough experience to level up.
        if (stats.CurrentExperience.Value >= stats.RequiredExperience.Value && IsServer)
        {
            LevelUp();
        }
    }

    void OnLevelChanged(int oldValue, int newValue)
    {
        if (IsOwner && oldValue > 0) stats.ScalingData.ApplyLevelUpGains(stats, newValue);
    }
}