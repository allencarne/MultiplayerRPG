using UnityEngine;
using UnityEngine.UI;

public class BeginnerSkillPanel : MonoBehaviour
{
    [SerializeField] PlayerAbilities playerAbilities;
    [SerializeField] Player player;

    [Header("Ability Bar")]
    [SerializeField] Image basic_BarImage;
    [SerializeField] Image offensive_BarImage;
    [SerializeField] Image mobility_BarImage;

    [Header("Basic")]
    [SerializeField] ScriptableObject basicAbility;
    [SerializeField] Image basic_MenuImage;

    [Header("Offensive 1")]
    [SerializeField] ScriptableObject offensiveAbility;
    [SerializeField] Image offensive_MenuImage;
    [SerializeField] Button offensive_Button;

    [Header("Offensive 2")]
    [SerializeField] ScriptableObject offensiveAbility2;
    [SerializeField] Image offensive2_MenuImage;
    [SerializeField] Button offensive2_Button;

    [Header("Mobility")]
    [SerializeField] ScriptableObject mobilityAbility;
    [SerializeField] Image mobility_MenuImage;
    [SerializeField] Button mobility_Button;

    private void Start()
    {
        Sprite basicIcon = (Sprite)basicAbility.GetType().GetField("icon").GetValue(basicAbility);

        if (basicIcon != null)
        {
            basic_MenuImage.sprite = basicIcon;
        }

        Sprite offensiveIcon = (Sprite)offensiveAbility.GetType().GetField("icon").GetValue(offensiveAbility);

        if (offensiveIcon != null)
        {
            offensive_MenuImage.sprite = offensiveIcon;
        }

        Sprite offensiveIcon2 = (Sprite)offensiveAbility2.GetType().GetField("icon").GetValue(offensiveAbility2);

        if (offensiveIcon2 != null)
        {
            offensive2_MenuImage.sprite = offensiveIcon2;
        }

        Sprite mobilityIcon = (Sprite)mobilityAbility.GetType().GetField("icon").GetValue(mobilityAbility);

        if (mobilityIcon != null)
        {
            mobility_MenuImage.sprite = mobilityIcon;
        }
    }

    private void Update()
    {
        EnableAbilities();
    }

    void EnableAbilities()
    {
        if (player.PlayerLevel < 5)
        {
            offensive_Button.interactable = true;
            offensive2_Button.interactable = true;
        }

        if (player.PlayerLevel < 10)
        {
            mobility_Button.interactable = true;
        }
    }

    public void BasicAbility_Button()
    {
        playerAbilities.SetBasicAbility(basicAbility);

        Sprite icon = (Sprite)basicAbility.GetType().GetField("icon").GetValue(basicAbility);

        if (icon != null)
        {
            basic_BarImage.sprite = icon;
        }
    }

    public void OffensiveAbility_Button()
    {
        playerAbilities.SetOffensiveAbility(offensiveAbility);

        Sprite icon = (Sprite)offensiveAbility.GetType().GetField("icon").GetValue(offensiveAbility);

        if (icon != null)
        {
            offensive_BarImage.sprite = icon;
        }
    }

    public void OffensiveAbility2_Button()
    {
        playerAbilities.SetOffensiveAbility(offensiveAbility2);

        Sprite icon = (Sprite)offensiveAbility2.GetType().GetField("icon").GetValue(offensiveAbility2);

        if (icon != null)
        {
            offensive_BarImage.sprite = icon;
        }
    }

    public void MobilityAbility_Button()
    {
        playerAbilities.SetMobilityAbility(mobilityAbility);

        Sprite icon = (Sprite)mobilityAbility.GetType().GetField("icon").GetValue(mobilityAbility);

        if (icon != null)
        {
            mobility_BarImage.sprite = icon;
        }
    }
}
