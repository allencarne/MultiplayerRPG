using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerExperience : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] TextPopUp textPopUp;
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI experienceText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] GameObject levelUpText;

    [Header("Components")]
    [SerializeField] Player player;
    [SerializeField] Image frontXpBar;
    [SerializeField] Image backXpBar;

    [Header("Multipliers")]
    [Range(1f, 300f)]
    public float additionMultiplier = 300;
    [Range(2f, 4f)]
    public float powerMultiplier = 2;
    [Range(7f, 14f)]
    public float divisionMultiplier = 7;

    public UnityEvent OnEXPGained;
    public UnityEvent OnLevelUp;

    public override void OnNetworkSpawn()
    {
        player.CurrentExperience.OnValueChanged += OnExperienceChanged;
        player.PlayerLevel.OnValueChanged += OnLevelChanged;

        frontXpBar.fillAmount = player.CurrentExperience.Value / player.RequiredExperience.Value;
        backXpBar.fillAmount = player.CurrentExperience.Value / player.RequiredExperience.Value;

        if (IsServer)
        {
            player.RequiredExperience.Value = CalculateRequiredXp();
        }

        levelText.text = player.PlayerLevel.Value.ToString();
        experienceText.text = player.CurrentExperience.Value + "/" + player.RequiredExperience.Value;
        StartCoroutine(LerpXpBar());
    }

    void OnExperienceChanged(float oldValue, float newValue)
    {
        StartCoroutine(LerpXpBar());

        experienceText.text = player.CurrentExperience.Value + "/" + player.RequiredExperience.Value;

        if (player.CurrentExperience.Value >= player.RequiredExperience.Value && IsServer)
        {
            LevelUp();
        }
    }

    void OnLevelChanged(int oldValue, int newValue)
    {
        levelText.text = player.PlayerLevel.Value.ToString();
    }

    IEnumerator LerpXpBar()
    {
        float elapsed = 0f;
        float duration = 1.5f;
        float startFill = frontXpBar.fillAmount;
        float targetFill = player.CurrentExperience.Value / player.RequiredExperience.Value;

        backXpBar.fillAmount = targetFill;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            frontXpBar.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        frontXpBar.fillAmount = targetFill;
    }

    private int CalculateRequiredXp()
    {
        int solveForRequiredXp = 0;
        for (int levelCycle = 1; levelCycle <= player.PlayerLevel.Value; levelCycle++)
        {
            solveForRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle / divisionMultiplier));
        }
        return solveForRequiredXp / 4;
    }

    public void IncreaseEXP(float xpGained)
    {
        if (!IsServer) return;

        player.CurrentExperience.Value += xpGained;

        textPopUp.EXPText(xpGained);
        OnEXPGained?.Invoke();
    }

    public void LevelUp()
    {
        // Increase Player Level
        player.PlayerLevel.Value++;

        // Attribute Points
        player.AttributePoints.Value += 5;

        // Increase Player Health
        player.MaxHealth.Value++;
        player.Health.Value++;

        // Increase Player Damage
        player.BaseDamage.Value++;
        player.CurrentDamage.Value++;

        // Update Bar
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        player.CurrentExperience.Value = Mathf.RoundToInt(player.CurrentExperience.Value - player.RequiredExperience.Value);
        player.RequiredExperience.Value = CalculateRequiredXp();

        OnLevelUp?.Invoke();
    }
}