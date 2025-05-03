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

    void Start()
    {
        frontXpBar.fillAmount = player.CurrentExperience / player.RequiredExperience;
        backXpBar.fillAmount = player.CurrentExperience / player.RequiredExperience;

        player.RequiredExperience = CalculateRequiredXp();

        levelText.text = player.PlayerLevel.ToString();
    }

    void Update()
    {
        UpdateXpUI();

        if (player.CurrentExperience >= player.RequiredExperience)
        {
            LevelUp();
        }
    }

    public void UpdateXpUI()
    {
        float xpFraction = player.CurrentExperience / player.RequiredExperience;
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

        experienceText.text = player.CurrentExperience + "/" + player.RequiredExperience;
    }

    private int CalculateRequiredXp()
    {
        int solveForRequiredXp = 0;
        for (int levelCycle = 1; levelCycle <= player.PlayerLevel; levelCycle++)
        {
            solveForRequiredXp += (int)Mathf.Floor(levelCycle + additionMultiplier * Mathf.Pow(powerMultiplier, levelCycle / divisionMultiplier));
        }
        return solveForRequiredXp / 4;
    }

    public void IncreaseEXP(float xpGained)
    {
        textPopUp.EXPText(xpGained);

        player.CurrentExperience += xpGained;
        lerpTimer = 0f;
        delayTimer = 0f;

        OnEXPGained?.Invoke();
    }

    public void LevelUp()
    {
        // Increase Player Level
        player.PlayerLevel++;
        levelText.text = player.PlayerLevel.ToString();

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
        player.CurrentExperience = Mathf.RoundToInt(player.CurrentExperience - player.RequiredExperience);
        player.RequiredExperience = CalculateRequiredXp();

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
