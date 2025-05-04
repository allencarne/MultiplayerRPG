using UnityEngine;
using UnityEngine.UI;

public class BeginnerSkillPanel : MonoBehaviour
{
    [SerializeField] Player player;

    [Header("Ability Bar")]
    [SerializeField] Image basic_BarImage;
    [SerializeField] Image offensive_BarImage;
    [SerializeField] Image mobility_BarImage;

    [Header("Basic")]
    [SerializeField] PlayerAbility basicAbility;
    [SerializeField] Image basic_MenuImage;

    [Header("Offensive 1")]
    [SerializeField] PlayerAbility offensiveAbility;
    [SerializeField] Image offensive_MenuImage;
    [SerializeField] Button offensive_Button;

    [Header("Offensive 2")]
    [SerializeField] PlayerAbility offensiveAbility2;
    [SerializeField] Image offensive2_MenuImage;
    [SerializeField] Button offensive2_Button;

    [Header("Mobility")]
    [SerializeField] PlayerAbility mobilityAbility;
    [SerializeField] Image mobility_MenuImage;
    [SerializeField] Button mobility_Button;

    private void Start()
    {
        basic_MenuImage.sprite = basicAbility.SkillIcon;
    }
}
