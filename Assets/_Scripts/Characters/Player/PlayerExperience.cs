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

    [Header("Multipliers")]
    [Range(1f, 300f)]
    public float additionMultiplier = 300;
    [Range(2f, 4f)]
    public float powerMultiplier = 2;
    [Range(7f, 14f)]
    public float divisionMultiplier = 7;

    [Header("Events")]
    public UnityEvent<float> OnEXPGained;
    public UnityEvent OnEXP;
    public UnityEvent OnLevelUp;

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

    int CalculateRequiredXp()
    {
        // Stores the total experience required.
        int solveForRequiredXp = 0;

        // Loop through every level from 1 to the player's current level.
        for (int levelCycle = 1; levelCycle <= stats.PlayerLevel.Value; levelCycle++)
        {
            // Calculate the experience required for this level.
            // The formula gradually increases faster as levels become higher.
            solveForRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle / divisionMultiplier));
        }

        // Scale the result down to make the numbers more reasonable.
        return solveForRequiredXp / 4;
    }

    IEnumerator LerpXpBar()
    {
        // Track how much time has passed.
        float elapsed = 0f;

        // Animation should take 1.5 seconds.
        float duration = 1.5f;

        // Remember where the bar is starting.
        float startFill = frontXpBar.fillAmount;

        // Calculate where the bar should end.
        float targetFill = stats.CurrentExperience.Value / stats.RequiredExperience.Value;

        // Instantly move the back bar to the target.
        backXpBar.fillAmount = targetFill;

        // Continue until the animation time has elapsed.
        while (elapsed < duration)
        {
            // Add the amount of time since the previous frame.
            elapsed += Time.deltaTime;

            // Convert elapsed time into a value between 0 and 1.
            float t = elapsed / duration;

            // Smoothly interpolate from the starting fill to the target fill.
            frontXpBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);

            // Wait until the next frame before continuing.
            yield return null;
        }

        // Ensure the bar finishes exactly at the target value.
        frontXpBar.fillAmount = targetFill;
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
        // Instantly update the front bar.
        frontXpBar.fillAmount = stats.CurrentExperience.Value / stats.RequiredExperience.Value;

        // Instantly update the back bar.
        backXpBar.fillAmount = stats.CurrentExperience.Value / stats.RequiredExperience.Value;

        // Refresh the displayed level.
        levelText.text = stats.PlayerLevel.Value.ToString();

        // Refresh the displayed experience numbers.
        experienceText.text = stats.CurrentExperience.Value + "/" + stats.RequiredExperience.Value;

        // Start animating the experience bar.
        StartCoroutine(LerpXpBar());

        // Mark that initialization has completed.
        statsInitialized = true;
    }

    void OnExperienceChanged(float oldValue, float newValue)
    {
        // Ignore experience changes until the UI has finished initializing.
        if (!statsInitialized) return;

        // Animate the experience bar toward the new value.
        StartCoroutine(LerpXpBar());

        // Update the displayed experience text.
        experienceText.text = stats.CurrentExperience.Value + "/" + stats.RequiredExperience.Value;

        // Check if we've reached enough experience to level up.
        if (stats.CurrentExperience.Value >= stats.RequiredExperience.Value && IsServer)
        {
            LevelUp();
        }
    }

    void OnLevelChanged(int oldValue, int newValue)
    {
        // Update the displayed level.
        levelText.text = newValue.ToString();

        // Only the owning player should receive bonus health.
        // Ignore the initial value (oldValue == 0) when the object first spawns.
        if (IsOwner && oldValue > 0)
        {
            // Give 2 health per level.
            int amount = newValue * 2;

            // Increase the player's health.
            stats.IncreaseHealth(amount);
        }
    }
}