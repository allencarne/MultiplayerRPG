using System.Collections.Generic;
using UnityEngine;

public class PlayerInitialize : MonoBehaviour
{
    [SerializeField] Player player;

    private void Start()
    {
        LoadPlayerStats();

        // Set Coin Text UI
        player.CoinCollected(0);
    }

    public void SavePlayerStats()
    {
        PlayerPrefs.SetInt("PlayerLevel", player.PlayerLevel);
        PlayerPrefs.SetFloat("CurrentExperience", player.CurrentExperience);
        PlayerPrefs.SetFloat("RequiredExperience", player.RequiredExperience);
        PlayerPrefs.SetFloat("Coins", player.Coins);

        PlayerPrefs.SetFloat("Health", player.Health);
        PlayerPrefs.SetFloat("MaxHealth", player.MaxHealth);

        PlayerPrefs.SetFloat("Speed", player.Speed);
        PlayerPrefs.SetFloat("CurrentSpeed", player.CurrentSpeed);

        PlayerPrefs.SetInt("Damage", player.Damage);
        PlayerPrefs.SetInt("CurrentDamage", player.CurrentDamage);

        PlayerPrefs.SetFloat("AttackSpeed", player.AttackSpeed);
        PlayerPrefs.SetFloat("CurrentAttackSpeed", player.CurrentAttackSpeed);

        PlayerPrefs.SetFloat("CDR", player.CDR);
        PlayerPrefs.SetFloat("CurrentCDR", player.CurrentCDR);

        PlayerPrefs.SetFloat("BaseArmor", player.BaseArmor);
        PlayerPrefs.SetFloat("CurrentArmor", player.CurrentArmor);

        PlayerPrefs.Save();
    }

    public void LoadPlayerStats()
    {
        player.PlayerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        player.CurrentExperience = PlayerPrefs.GetFloat("CurrentExperience", 0);
        player.RequiredExperience = PlayerPrefs.GetFloat("RequiredExperience", 10);
        player.Coins = PlayerPrefs.GetFloat("Coins", 0);

        player.Health = PlayerPrefs.GetFloat("Health",10);
        player.MaxHealth = PlayerPrefs.GetFloat("MaxHealth", 10);

        player.Speed = PlayerPrefs.GetFloat("Speed", 5);
        player.CurrentSpeed = PlayerPrefs.GetFloat("CurrentSpeed", 5);

        player.Damage = PlayerPrefs.GetInt("Damage", 1);
        player.CurrentDamage = PlayerPrefs.GetInt("CurrentDamage", 1);

        player.AttackSpeed = PlayerPrefs.GetFloat("AttackSpeed", 1);
        player.CurrentAttackSpeed = PlayerPrefs.GetFloat("CurrentAttackSpeed", 1);

        player.CDR = PlayerPrefs.GetFloat("CDR", 1);
        player.CurrentCDR = PlayerPrefs.GetFloat("CurrentCDR", 1);

        player.BaseArmor = PlayerPrefs.GetFloat("BaseArmor", 0);
        player.CurrentArmor = PlayerPrefs.GetFloat("CurrentArmor", 0);
    }
}
