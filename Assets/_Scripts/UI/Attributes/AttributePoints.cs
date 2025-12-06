using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttributePoints : NetworkBehaviour
{
    [SerializeField] Color defaultColor;
    [Header("Button")]
    [SerializeField] GameObject buttonToSelect;
    [SerializeField] Button applyButton;

    [Header("Player")]
    [SerializeField] PlayerStats stats;

    [Header("Health")]
    [SerializeField] Button HealthPlus;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Button HealthMinus;

    [Header("Damage")]
    [SerializeField] Button DamagePlus;
    [SerializeField] TextMeshProUGUI DamageText;
    [SerializeField] Button DamageMinus;

    [Header("Attack Speed")]
    [SerializeField] Button ASPlus;
    [SerializeField] TextMeshProUGUI ASText;
    [SerializeField] Button ASMinus;

    [Header("Cool Down Reduction")]
    [SerializeField] Button CRDPlus;
    [SerializeField] TextMeshProUGUI CDRText;
    [SerializeField] Button CDRMinus;

    int healthToAdd;
    int damageToAdd;
    int asToAdd;
    int cdrToAdd;

    public UnityEvent OnStatsApplied;

    private void Start()
    {
        HideText();
    }

    private void Update()
    {
        int TotalToAdd = healthToAdd + damageToAdd + asToAdd + cdrToAdd;

        if (TotalToAdd != 0)
        {
            applyButton.interactable = true;
        }
        else
        {
            applyButton.interactable = false;
        }

        ColorBlock colors = HealthPlus.colors;
        colors.normalColor = stats.AttributePoints.Value != 0 ? Color.cyan : defaultColor;
        HealthPlus.colors = colors;
        HealthMinus.colors = colors;
        DamagePlus.colors = colors;
        DamageMinus.colors = colors;
        ASPlus.colors = colors;
        ASMinus.colors = colors;
        CRDPlus.colors = colors;
        CDRMinus.colors = colors;
    }

    public void PlusButton(int stat)
    {
        int TotalToAdd = healthToAdd + damageToAdd + asToAdd + cdrToAdd;

        if (stats.AttributePoints.Value <= 0) return;
        if (TotalToAdd >= stats.AttributePoints.Value) return;

        switch (stat)
        {
            case 0:
                healthToAdd++;
                healthText.text = healthToAdd.ToString();
                break;
            case 1:
                damageToAdd++;
                DamageText.text = damageToAdd.ToString();
                break;
            case 2:
                asToAdd++;
                ASText.text = asToAdd.ToString();
                break;
            case 3:
                cdrToAdd++;
                CDRText.text = cdrToAdd.ToString();
                break;
        }

        HideText();
    }

    public void MinusButton(int stat)
    {
        switch (stat)
        {
            case 0:
                if (healthToAdd == 0) return;
                healthToAdd--;
                healthText.text = healthToAdd.ToString();
                break;
            case 1:
                if (damageToAdd == 0) return;
                damageToAdd--;
                DamageText.text = damageToAdd.ToString();
                break;
            case 2:
                if (asToAdd == 0) return;
                asToAdd--;
                ASText.text = asToAdd.ToString();
                break;
            case 3:
                if (cdrToAdd == 0) return;
                cdrToAdd--;
                CDRText.text = cdrToAdd.ToString();
                break;
        }

        HideText();
    }

    public void ApplyButton()
    {
        stats.IncreaseHealth(healthToAdd);
        stats.IncreaseDamage(damageToAdd);
        stats.IncreaseAttackSpeed(asToAdd);
        stats.IncreaseCoolDownReduction(cdrToAdd);
        ConsumeAttributePoints(healthToAdd + damageToAdd + asToAdd + cdrToAdd);

        healthToAdd = 0;
        healthText.text = healthToAdd.ToString();

        damageToAdd = 0;
        DamageText.text = damageToAdd.ToString();

        asToAdd = 0;
        ASText.text = asToAdd.ToString();

        cdrToAdd = 0;
        CDRText.text = cdrToAdd.ToString();

        OnStatsApplied?.Invoke();

        EventSystem.current.SetSelectedGameObject(buttonToSelect);
    }

    void HideText()
    {
        if (damageToAdd < 1) DamageText.text = "";
        if (healthToAdd < 1) healthText.text = "";
        if (asToAdd < 1) ASText.text = "";
        if (cdrToAdd < 1) CDRText.text = "";
    }

    public void ConsumeAttributePoints(int amount)
    {
        if (IsServer)
        {
            stats.AttributePoints.Value -= amount;
        }
        else
        {
            ConsumeAttributePointsServerRPC(amount);
        }
    }

    [ServerRpc]
    void ConsumeAttributePointsServerRPC(int amount)
    {
        stats.AttributePoints.Value -= amount;
    }
}
