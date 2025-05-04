using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    [SerializeField] Player player;

    [Header("Ability Bar")]
    [SerializeField] Image skillBar_Basic;
    [SerializeField] Image skillbar_Offensive;
    [SerializeField] Image skillbar_Mobility;

    [Header("Ability")]
    public PlayerAbility[] basicAbilities;
    public PlayerAbility[] offensiveAbilities;
    public PlayerAbility[] mobilityAbilities;
    public PlayerAbility[] defensiveAbilities;
    public PlayerAbility[] utilityAbilities;
    public PlayerAbility[] ultimateAbilities;

    [Header("Basic")]
    [SerializeField] Image icon_Basic;
    [SerializeField] Image icon_Basic2;
    [SerializeField] Image icon_Basic3;
    [Header("Offensive 1")]
    [SerializeField] Image icon_Offensive;
    [SerializeField] Image icon_Offensiv2;
    [SerializeField] Image icon_Offensiv3;
    [Header("Mobility")]
    [SerializeField] Image icon_Mobility;
    [SerializeField] Image icon_Mobility2;
    [SerializeField] Image icon_Mobility3;

    private void Start()
    {
        icon_Basic.sprite = basicAbilities[player.BasicIndex].SkillIcon;
    }
}
