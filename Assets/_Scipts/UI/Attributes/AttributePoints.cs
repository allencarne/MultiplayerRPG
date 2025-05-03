using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AttributePoints : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Button ApplyButton;
    [SerializeField] TextMeshProUGUI apText;

    [SerializeField] Button HealthPlus;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Button HealthMinus;

    [SerializeField] Button DamagePlus;
    [SerializeField] TextMeshProUGUI DamageText;
    [SerializeField] Button DamageMinus;

    [SerializeField] Button ASPlus;
    [SerializeField] TextMeshProUGUI ASText;
    [SerializeField] Button ASMinus;

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
        ApplyButton.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (player.AttributePoints != 0)
        {
            ApplyButton.gameObject.SetActive(true);
        }
        else
        {
            ApplyButton.gameObject.SetActive(false);
        }

        ColorBlock colors = HealthPlus.colors; // Copy the struct
        colors.normalColor = player.AttributePoints != 0 ? Color.yellow : Color.white;
        HealthPlus.colors = colors;
        HealthMinus.colors = colors;
        DamagePlus.colors = colors;
        DamageMinus.colors = colors;
        ASPlus.colors = colors;
        ASMinus.colors = colors;
        CRDPlus.colors = colors;
        CDRMinus.colors = colors;
    }

    public void PlusButtonPressed()
    {
        if (player.AttributePoints <= 0) return;
        if (healthToAdd >= player.AttributePoints) return;

        healthToAdd++;
        healthText.text = healthToAdd.ToString();
    }

    public void MinusButtonPressed()
    {
        if (healthToAdd == 0) return;

        healthToAdd--;
        healthText.text = healthToAdd.ToString();
    }

    public void ApplyButtonPressed()
    {
        player.MaxHealth.Value += healthToAdd;
        player.AttributePoints -= healthToAdd;
        healthToAdd = 0;

        healthText.text = healthToAdd.ToString();
        apText.text = player.AttributePoints.ToString();

        OnStatsApplied?.Invoke();
    }
}
