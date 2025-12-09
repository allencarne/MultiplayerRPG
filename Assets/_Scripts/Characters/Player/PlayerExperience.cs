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
    [SerializeField] PlayerStats stats;
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
        stats.CurrentExperience.OnValueChanged += OnExperienceChanged;
        stats.PlayerLevel.OnValueChanged += OnLevelChanged;
    }

    private void OnDisable()
    {
        stats.CurrentExperience.OnValueChanged -= OnExperienceChanged;
        stats.PlayerLevel.OnValueChanged -= OnLevelChanged;
    }

    private void Start()
    {
        Invoke("Init", 3);
    }

    void Init()
    {
        if (IsServer)
        {
            stats.RequiredExperience.Value = CalculateRequiredXp();
        }

        frontXpBar.fillAmount = stats.CurrentExperience.Value / stats.RequiredExperience.Value;
        backXpBar.fillAmount = stats.CurrentExperience.Value / stats.RequiredExperience.Value;

        levelText.text = stats.PlayerLevel.Value.ToString();
        experienceText.text = stats.CurrentExperience.Value + "/" + stats.RequiredExperience.Value;
        StartCoroutine(LerpXpBar());
    }

    void OnExperienceChanged(float oldValue, float newValue)
    {
        StartCoroutine(LerpXpBar());

        experienceText.text = stats.CurrentExperience.Value + "/" + stats.RequiredExperience.Value;

        if (stats.CurrentExperience.Value >= stats.RequiredExperience.Value && IsServer)
        {
            LevelUp();
        }
    }

    void OnLevelChanged(int oldValue, int newValue)
    {
        levelText.text = stats.PlayerLevel.Value.ToString();
    }

    IEnumerator LerpXpBar()
    {
        float elapsed = 0f;
        float duration = 1.5f;
        float startFill = frontXpBar.fillAmount;
        float targetFill = stats.CurrentExperience.Value / stats.RequiredExperience.Value;

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

    int CalculateRequiredXp()
    {
        int solveForRequiredXp = 0;
        for (int levelCycle = 1; levelCycle <= stats.PlayerLevel.Value; levelCycle++)
        {
            solveForRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle / divisionMultiplier));
        }
        return solveForRequiredXp / 4;
    }

    public void IncreaseEXP(float xpGained)
    {
        if (IsServer)
        {
            stats.CurrentExperience.Value += xpGained;
        }
        else
        {
            IncreaseEXPServerRPC(xpGained);
        }

        OnEXPGained?.Invoke();
        textPopUp.EXPText(xpGained);
    }

    [ServerRpc]
    void IncreaseEXPServerRPC(float xpGained)
    {
        stats.CurrentExperience.Value += xpGained;
    }

    void LevelUp()
    {
        // Increase Player Level
        stats.PlayerLevel.Value++;

        // Attribute Points
        if (stats.PlayerLevel.Value < 10)
        {
            stats.AttributePoints.Value += 1;
        }
        else
        {
            stats.AttributePoints.Value += 3;
        }

        // Increase Player Health
        //player.MaxHealth.Value++;
        //player.Health.Value++;

        // Increase Player Damage
        //player.BaseDamage.Value++;
        //player.CurrentDamage.Value++;

        stats.GiveHeal(100, HealType.Percentage);

        // Update Bar
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        stats.CurrentExperience.Value = Mathf.RoundToInt(stats.CurrentExperience.Value - stats.RequiredExperience.Value);
        stats.RequiredExperience.Value = CalculateRequiredXp();

        OnLevelUp?.Invoke();
    }
}