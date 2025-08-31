using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttributePoints : MonoBehaviour
{
    [SerializeField] Color defaultColor;
    [Header("Button")]
    [SerializeField] GameObject buttonToSelect;
    [SerializeField] Button applyButton;

    [Header("Player")]
    [SerializeField] Player player;

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

        ColorBlock colors = HealthPlus.colors; // Copy the struct
        colors.normalColor = player.AttributePoints.Value != 0 ? Color.cyan : defaultColor;
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

        if (player.AttributePoints.Value <= 0) return;
        if (TotalToAdd >= player.AttributePoints.Value) return;

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
    }

    public void ApplyButton()
    {
        player.IncreaseHealth(healthToAdd);
        player.IncreaseDamage(damageToAdd);
        player.IncreaseAttackSpeed(asToAdd * 0.1f);
        player.IncreaseCoolDown(cdrToAdd * 0.1f);
        player.ConsumeAttributePoints(healthToAdd + damageToAdd + asToAdd + cdrToAdd);

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
}
