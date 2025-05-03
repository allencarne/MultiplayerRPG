using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AttributePoints : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Button ApplyButton;

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
    [SerializeField] Button CRDMinus;

    private void Start()
    {
        ApplyButton.enabled = false;
    }

    private void Update()
    {
        if (player.AttributePoints != 0)
        {
            ApplyButton.enabled = true;
        }

        var colors = HealthPlus.colors; // Copy the struct
        colors.normalColor = player.AttributePoints != 0 ? Color.yellow : Color.white;
        HealthPlus.colors = colors; // Re-assign the modified copy
    }

    public void PlusButton()
    {

    }

    public void MinusButton()
    {

    }
}
