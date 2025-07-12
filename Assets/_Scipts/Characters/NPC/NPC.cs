using Unity.Netcode;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float StartingMaxHealth;
    public NetworkVariable<float> Health = new(writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<float> MaxHealth = new(writePerm: NetworkVariableWritePermission.Server);
    [Header("Speed")]
    public float BaseSpeed;
    public float CurrentSpeed;
    [Header("Damage")]
    public int BaseDamage;
    public int CurrentDamage;
    [Header("AttackSpeed")]
    public float BaseAttackSpeed;
    public float CurrentAttackSpeed;
    [Header("CDR")]
    public float BaseCDR;
    public float CurrentCDR;
    [Header("Armor")]
    public float BaseArmor;
    public float CurrentArmor;

    // Components

}
