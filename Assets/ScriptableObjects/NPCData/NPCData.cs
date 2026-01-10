using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Scriptable Objects/NPCData")]
public class NPCData : ScriptableObject
{
    [Header("Info")]
    public string NPC_ID;
    public string NPCName;
    public int NPC_Level;
    public NPCClass npcClass;

    [Header("Stats")]
    public int MaxHealth;
    public int Damage;
    public int Speed;
    public int AttackSpeed;
    public int CoolDownRecution;
    public float Armor;

    public float TotalPatience;

    [Header("Customization")]
    public int skinColorIndex;

    public int hairStyleIndex;
    public int hairColorIndex;

    public int eyeStyleIndex;
    public int eyeColorIndex;

    [Header("Equipment")]
    public int HelmIndex;
    public int ChestIndex;
    public int LegsIndex;
    public Sprite Weapon;

    [Header("Quest")]
    public List<Quest> Quests;
    [TextArea(3, 8)] public string[] Dialogue;
    public Item[] Items;
}

public enum NPCClass
{
    Quest,
    Vendor,
    Guard,
    Patrol,
    Villager
}