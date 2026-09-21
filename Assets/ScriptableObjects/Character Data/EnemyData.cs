using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Character/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyScailingData Scaling;

    public string Enemy_ID;
    public string Enemy_Name;
    public int Enemy_Level;
    public EnemyType Enemy_Type;

    public float ExpToGive => Scaling.GetExp(Enemy_Level, Enemy_Type);
    public float TotalPatience;

    [Header("Stats")]
    public float StartingHealth => Scaling.GetHealth(Enemy_Level, Enemy_Type);
    public float StartingDamage => Scaling.GetDamage(Enemy_Level, Enemy_Type);
    public float StartingAS;
    public float StartingCDR;
    public float StartingSpeed;
    public float StartingArmor => Scaling.GetArmor(Enemy_Level, Enemy_Type);
    public float StartingHealthRegeneration;

    [Header("Enemy Radius")]
    public float WanderRadius;
    public float DeAggroRadius;

    [Header("Combat Radius")]
    public float BasicRadius;
    public float SpecialRadius;
    public float UltimateRadius;

    [Header("Drops")]
    public Item[] DroppableItems;

    [Header("Skills")]
    public PassiveSkillData PassiveAbility;
    public ActiveSkillData BasicAbility;
    public ActiveSkillData SpecialAbility;
    public ActiveSkillData UltimateAbility;
}

public enum EnemyType
{
    Enemy,
    Giant,
    Dummy
}
