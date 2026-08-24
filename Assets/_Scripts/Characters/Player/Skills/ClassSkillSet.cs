using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Character/Class Skill Set")]
public class ClassSkillSet : ScriptableObject
{
    [Header("Passives")]
    public PassiveSkillData[] firstPassive;
    public PassiveSkillData[] secondPassive;
    public PassiveSkillData[] thirdPassive;

    [Header("Actives")]
    public ActiveSkillData[] basicAbilities;
    public ActiveSkillData[] offensiveAbilities;
    public ActiveSkillData[] mobilityAbilities;
    public ActiveSkillData[] defensiveAbilities;
    public ActiveSkillData[] utilityAbilities;
    public ActiveSkillData[] ultimateAbilities;

    [Header("Level Requirements")]
    [HideInInspector] public int passive1Req = 0;
    [HideInInspector] public int basicReq = 0;
    [HideInInspector] public int offensiveReq = 4;
    [HideInInspector] public int passive2Req = 6;
    [HideInInspector] public int mobilityReq = 8;
    [HideInInspector] public int defensiveReq = 12;
    [HideInInspector] public int passive3Req = 14;
    [HideInInspector] public int utilityReq = 16;
    [HideInInspector] public int ultimateReq = 20;
}
