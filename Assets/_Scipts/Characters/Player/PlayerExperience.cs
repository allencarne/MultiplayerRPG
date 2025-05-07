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

    [Header("Particles")]
    [SerializeField] GameObject levelUpParticle;
    [SerializeField] GameObject levelUpParticle_Back;

    [Header("Variables")]
    private float lerpTimer;
    private float delayTimer;

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

        frontXpBar.fillAmount = player.CurrentExperience.Value / player.RequiredExperience.Value;
        backXpBar.fillAmount = player.CurrentExperience.Value / player.RequiredExperience.Value;

        levelText.text = player.PlayerLevel.Value.ToString();

        if (IsServer)
        {
            player.RequiredExperience.Value = CalculateRequiredXp();
        }

        UpdateXpUI();
    }

    void OnExperienceChanged(float oldValue, float newValue)
    {
        UpdateXpUI();

        if (player.CurrentExperience.Value >= player.RequiredExperience.Value && IsServer)
        {
            LevelUp();
        }
    }

    public void UpdateXpUI()
    {
        float xpFraction = player.CurrentExperience.Value / player.RequiredExperience.Value;
        float FXP = frontXpBar.fillAmount;
        if (FXP < xpFraction)
        {
            delayTimer += Time.deltaTime;
            backXpBar.fillAmount = xpFraction;
            if (delayTimer > 0.5)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / 4;
                frontXpBar.fillAmount = Mathf.Lerp(FXP, backXpBar.fillAmount, percentComplete);
            }
        }

        experienceText.text = player.CurrentExperience.Value + "/" + player.RequiredExperience.Value;
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
        lerpTimer = 0f;
        delayTimer = 0f;

        textPopUp.EXPText(xpGained);
        OnEXPGained?.Invoke();
    }

    public void LevelUp()
    {
        // Increase Player Level
        player.PlayerLevel.Value++;
        levelText.text = player.PlayerLevel.Value.ToString();

        //Stats
        // Attribute Points
        player.AttributePoints += 5;

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

        // Effects
        SpawnEffectClientRPC();

        OnLevelUp?.Invoke();
    }

    [ClientRpc]
    void SpawnEffectClientRPC()
    {
        GameObject text = Instantiate(levelUpText, rect.transform.position, Quaternion.identity, rect.transform);
        GameObject effect = Instantiate(levelUpParticle, transform.position, Quaternion.identity, transform);
        GameObject effect_back = Instantiate(levelUpParticle_Back, transform.position, Quaternion.identity, transform);

        Destroy(text, 3);
        Destroy(effect, 2);
        Destroy(effect_back, 2);
    }
}