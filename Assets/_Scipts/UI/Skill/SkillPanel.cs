using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    [SerializeField] Player player;

    [Header("Ability")]
    public PlayerAbility[] basicAbilities;
    public PlayerAbility[] offensiveAbilities;
    public PlayerAbility[] mobilityAbilities;
    public PlayerAbility[] defensiveAbilities;
    public PlayerAbility[] utilityAbilities;
    public PlayerAbility[] ultimateAbilities;

    [Header("Basic")]
    [SerializeField] Image icon_Basic0;
    [SerializeField] Image icon_Basic1;
    [SerializeField] Image icon_Basic2;
    [Header("Offensive 1")]
    [SerializeField] Image icon_Offensive0;
    [SerializeField] Image icon_Offensive1;
    [SerializeField] Image icon_Offensive2;
    [Header("Mobility")]
    [SerializeField] Image icon_Mobility0;
    [SerializeField] Image icon_Mobility1;
    [SerializeField] Image icon_Mobility2;
    [Header("Defensive")]
    [SerializeField] Image icon_Defensive0;
    [SerializeField] Image icon_Defensive1;
    [SerializeField] Image icon_Defensive2;
    [Header("Utility")]
    [SerializeField] Image icon_Utility0;
    [SerializeField] Image icon_Utility1;
    [SerializeField] Image icon_Utility2;
    [Header("Ultimate")]
    [SerializeField] Image icon_Ultimate0;
    [SerializeField] Image icon_Ultimate1;
    [SerializeField] Image icon_Ultimate2;

    private void Start()
    {
        AssignIcon(icon_Basic0, basicAbilities, 0);
        AssignIcon(icon_Basic1, basicAbilities, 1);
        AssignIcon(icon_Basic2, basicAbilities, 2);

        AssignIcon(icon_Offensive0, offensiveAbilities, 0);
        AssignIcon(icon_Offensive1, offensiveAbilities, 1);
        AssignIcon(icon_Offensive2, offensiveAbilities, 2);

        AssignIcon(icon_Mobility0, mobilityAbilities, 0);
        AssignIcon(icon_Mobility1, mobilityAbilities, 1);
        AssignIcon(icon_Mobility2, mobilityAbilities, 2);
    }

    private void AssignIcon(Image icon, PlayerAbility[] abilities, int index)
    {
        if (icon == null || abilities == null || index >= abilities.Length || abilities[index] == null)
            return;

        if (abilities[index].SkillIcon != null)
            icon.sprite = abilities[index].SkillIcon;
    }
}
