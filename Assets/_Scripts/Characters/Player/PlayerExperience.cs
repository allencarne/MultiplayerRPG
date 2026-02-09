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

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.F3))
        {
            IncreaseEXP(1);
        }

        if (Input.GetKeyDown(KeyCode.F4))
        {
            IncreaseEXP(10);
        }
    }

    public override void OnNetworkSpawn()
    {
        stats.RequiredExperience.OnValueChanged += OnReqExperienceChanged;
        stats.CurrentExperience.OnValueChanged += OnExperienceChanged;
        stats.PlayerLevel.OnValueChanged += OnLevelChanged;
    }

    private void OnDisable()
    {
        stats.RequiredExperience.OnValueChanged -= OnReqExperienceChanged;
        stats.CurrentExperience.OnValueChanged -= OnExperienceChanged;
        stats.PlayerLevel.OnValueChanged -= OnLevelChanged;
    }

    public void Initialize()
    {
        if (IsServer)
        {
            stats.RequiredExperience.Value = CalculateRequiredXp();
        }
        else
        {
            CalculateServerRPC();
        }
    }

    [ServerRpc]
    void CalculateServerRPC()
    {
        stats.RequiredExperience.Value = CalculateRequiredXp();
    }

    void OnReqExperienceChanged(float oldValue, float newValue)
    {
        frontXpBar.fillAmount = stats.CurrentExperience.Value / stats.RequiredExperience.Value;
        backXpBar.fillAmount = stats.CurrentExperience.Value / stats.RequiredExperience.Value;

        levelText.text = stats.PlayerLevel.Value.ToString();
        experienceText.text = stats.CurrentExperience.Value + "/" + stats.RequiredExperience.Value;
        StartCoroutine(LerpXpBar());

        statsInitialized = true;
    }

    void OnExperienceChanged(float oldValue, float newValue)
    {
        if (!statsInitialized) return;

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

        OnEXPGained?.Invoke(xpGained);
        OnEXP?.Invoke();
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

        // Increase Player Health
        int amount = stats.PlayerLevel.Value * 2;
        stats.IncreaseHealth(amount);

        // Attribute Points
        stats.IncreaseAttribuePoints();

        stats.GiveHeal(100, HealType.Percentage);

        // Update Bar
        frontXpBar.fillAmount = 0f;
        backXpBar.fillAmount = 0f;
        stats.CurrentExperience.Value = Mathf.RoundToInt(stats.CurrentExperience.Value - stats.RequiredExperience.Value);
        stats.RequiredExperience.Value = CalculateRequiredXp();

        OnLevelUp?.Invoke();
    }
}