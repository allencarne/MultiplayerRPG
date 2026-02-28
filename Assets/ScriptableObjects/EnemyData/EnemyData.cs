using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Character/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string Enemy_ID;
    public string Enemy_Name;
    public int Enemy_Level;

    public float ExpToGive;
    public float TotalPatience;

    [Header("Stats")]
    public float StartingHealth;
    public float StartingDamage;
    public float StartingAS;
    public float StartingCDR;
    public float StartingSpeed;
    public float StartingArmor;

    [Header("Enemy Radius")]
    public float WanderRadius;
    public float DeAggroRadius;

    [Header("Combat Radius")]
    public float BasicRadius;
    public float SpecialRadius;
    public float UltimateRadius;
}
