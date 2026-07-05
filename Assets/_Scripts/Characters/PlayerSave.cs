using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSave : NetworkBehaviour
{
    [Header("Data")]
    [SerializeField] CharacterCustomizationData customizationData;
    [SerializeField] QuestList questList;
    bool statsInitialized;

    [Header("References")]
    Player player;
    PlayerStateMachine stateMachine;
    PlayerCustomization custom;
    PlayerStats stats;
    Inventory inventory;
    EquipmentManager equipment;
    [SerializeField] AttributePoints ap;
    [SerializeField] PlayerExperience exp;
    [SerializeField] SkillPanel skillPanel;
    [SerializeField] PlayerQuest playerQuest;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI saveText;

    private void Awake()
    {
        player = GetComponent<Player>();
        stateMachine = GetComponent<PlayerStateMachine>();
        custom = GetComponent<PlayerCustomization>();
        stats = GetComponent<PlayerStats>();
        inventory = GetComponentInChildren<Inventory>();
        equipment = GetComponentInChildren<EquipmentManager>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetSelectedCharacterServerRpc(PlayerPrefs.GetInt("SelectedCharacter"));

            LoadCustomization();
            LoadPlayerStats();
            LoadCharacterStats();
            LoadPlayerSkills();
            LoadInventory();
            equipment.LoadEquipment();
            exp.Initialize();
            stateMachine.SkillsOnSpawn();
            LoadQuests();

            statsInitialized = true;

            playerQuest.OnQuestStateChanged.AddListener(SaveQuests);
            playerQuest.OnQuestTurnedIn.AddListener(SaveQuestsOnTurnIn);
        }
        else
        {
            switch (stats.playerClass)
            {
                case PlayerStats.PlayerClass.Beginner:
                    custom.playerNameText.text = $"<sprite name=\"Beginner_Icon\"> {stats.net_playerName.Value}";
                    break;
                case PlayerStats.PlayerClass.Warrior:
                    custom.playerNameText.text = $"<sprite name=\"Warrior_Icon\"> {stats.net_playerName.Value}";
                    break;
                case PlayerStats.PlayerClass.Magician:
                    custom.playerNameText.text = $"<sprite name=\"Magician_Icon\"> {stats.net_playerName.Value}";
                    break;
                case PlayerStats.PlayerClass.Archer:
                    custom.playerNameText.text = $"<sprite name=\"Archer_Icon\"> {stats.net_playerName.Value}";
                    break;
                case PlayerStats.PlayerClass.Rogue:
                    custom.playerNameText.text = $"<sprite name=\"Rogue_Icon\"> {stats.net_playerName.Value}";
                    break;
            }
            custom.bodySprite.color = stats.net_bodyColor.Value;
            custom.playerHeadSprite.color = stats.net_bodyColor.Value;
            custom.hairSprite.color = stats.net_hairColor.Value;

            Material eyeMat = custom.eyesSprite.material;
            eyeMat.SetColor("_NewColor", stats.net_eyeColor.Value);
        }

        ap.OnStatsApplied.AddListener(SaveStats);
        exp.OnEXP.AddListener(SaveStats);
        skillPanel.OnSkillSelected.AddListener(SaveStats);
    }

    public override void OnNetworkDespawn()
    {
        ap.OnStatsApplied.RemoveListener(SaveStats);
        exp.OnEXP.RemoveListener(SaveStats);
        skillPanel.OnSkillSelected.RemoveListener(SaveStats);

        playerQuest.OnQuestStateChanged.RemoveListener(SaveQuests);
        playerQuest.OnQuestTurnedIn.RemoveListener(SaveQuestsOnTurnIn);
    }

    [ServerRpc]
    void SetSelectedCharacterServerRpc(int slot)
    {
        stats.net_CharacterSlot.Value = slot;
    }

    void LoadCustomization()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        string name = PlayerPrefs.GetString($"Character{slot}Name", "No Name");
        int hairIndex = PlayerPrefs.GetInt($"Character{slot}HairStyle");
        int eyeIndex = PlayerPrefs.GetInt($"Character{slot}EyeStyle");
        Color skinCol = customizationData.skinColors[PlayerPrefs.GetInt($"Character{slot}SkinColor")];
        Color hairCol = customizationData.hairColors[PlayerPrefs.GetInt($"Character{slot}HairColor")];
        Color eyeCol = customizationData.eyeColors[PlayerPrefs.GetInt($"Character{slot}EyeColor")];

        switch (stats.playerClass)
        {
            case PlayerStats.PlayerClass.Beginner:
                custom.playerNameText.text = $"<sprite name=\"Beginner_Icon\"> {name}";
                break;
            case PlayerStats.PlayerClass.Warrior:
                custom.playerNameText.text = $"<sprite name=\"Warrior_Icon\"> {name}";
                break;
            case PlayerStats.PlayerClass.Magician:
                custom.playerNameText.text = $"<sprite name=\"Magician_Icon\"> {name}";
                break;
            case PlayerStats.PlayerClass.Archer:
                custom.playerNameText.text = $"<sprite name=\"Archer_Icon\"> {name}";
                break;
            case PlayerStats.PlayerClass.Rogue:
                custom.playerNameText.text = $"<sprite name=\"Rogue_Icon\"> {name}";
                break;
        }

        custom.bodySprite.color = skinCol;
        custom.playerHeadSprite.color = skinCol;
        custom.hairSprite.color = hairCol;

        Material eyeMat = custom.eyesSprite.material;
        eyeMat.SetColor("_NewColor", eyeCol);

        if (IsServer)
        {
            ApplyCustomization(slot, name, skinCol, hairCol, eyeCol, hairIndex, eyeIndex);
        }
        else
        {
            LoadCustomizationServerRPC(slot, name, skinCol, hairCol, eyeCol, hairIndex, eyeIndex);
        }
    }

    void ApplyCustomization(int slot, FixedString32Bytes name, Color skin, Color hair, Color eye, int hairIndex, int eyeIndex)
    {
        stats.net_CharacterSlot.Value = slot;
        stats.net_playerName.Value = name;
        stats.net_bodyColor.Value = skin;
        stats.net_hairColor.Value = hair;
        stats.net_eyeColor.Value = eye;

        custom.net_HairIndex.Value = hairIndex;
        custom.net_EyeIndex.Value = eyeIndex;
    }

    [ServerRpc]
    void LoadCustomizationServerRPC(int slot, FixedString32Bytes name, Color skin, Color hair, Color eye, int hairIndex, int eyeIndex)
    {
        ApplyCustomization(slot, name, skin, hair, eye, hairIndex, eyeIndex);
    }

    void LoadPlayerStats()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        int level = PlayerPrefs.GetInt($"{slot}PlayerLevel", 1);
        int playerClass = PlayerPrefs.GetInt($"{slot}PlayerClass", 0);
        float currentExp = PlayerPrefs.GetFloat($"{slot}CurrentExperience", 0);
        int ap = PlayerPrefs.GetInt($"{slot}AP", 0);
        float coins = PlayerPrefs.GetFloat($"{slot}Coins", 0);

        stats.playerClass = (PlayerStats.PlayerClass)playerClass;

        if (IsServer)
        {
            ApplyPlayerStats(level, currentExp, coins, ap);
        }
        else
        {
            LoadPlayerStatsServerRPC(level, currentExp, coins, ap);
        }
    }

    void ApplyPlayerStats(int level, float cEXP, float coins, int ap)
    {
        stats.PlayerLevel.Value = level;
        stats.CurrentExperience.Value = cEXP;
        stats.AttributePoints.Value = ap;
        stats.Coins = coins;
    }

    [ServerRpc]
    void LoadPlayerStatsServerRPC(int level, float cEXP, float coins, int ap)
    {
        ApplyPlayerStats(level, cEXP, coins, ap);
    }

    void LoadCharacterStats()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        float health = PlayerPrefs.GetFloat($"{slot}MaxHealth", 10);
        float damage = PlayerPrefs.GetFloat($"{slot}Damage", 1);
        float attackSpeed = PlayerPrefs.GetFloat($"{slot}AttackSpeed", 1);
        float cdr = PlayerPrefs.GetFloat($"{slot}CDR", 1);
        float armor = PlayerPrefs.GetFloat($"{slot}Armor", 0);
        float speed = PlayerPrefs.GetFloat($"{slot}Speed", 5);
        float fury = PlayerPrefs.GetFloat($"{slot}MaxFury", 100);
        float end = PlayerPrefs.GetFloat($"{slot}MaxEndurance", 100);
        float endrech = PlayerPrefs.GetFloat($"{slot}EnduranceRecharge", 1);

        if (IsServer)
        {
            ApplyCharacterStats(health, damage, attackSpeed, cdr, armor, speed, fury, end, endrech);
        }
        else
        {
            LoadCharacterStatsServerRPC(health, damage, attackSpeed, cdr, armor, speed, fury, end, endrech);
        }
    }

    void ApplyCharacterStats(float health, float damage, float attackSpeed, float cdr, float armor, float speed, float fury, float end, float endrech)
    {
        stats.net_BaseHP.Value = health;
        stats.net_BaseDamage.Value = damage;
        stats.net_BaseAS.Value = attackSpeed;
        stats.net_BaseCDR.Value = cdr;
        stats.net_BaseArmor.Value = armor;
        stats.net_BaseSpeed.Value = speed;
        stats.MaxFury.Value = fury;
        stats.MaxEndurance.Value = end;
        stats.EnduranceRechargeRate.Value = endrech;

        stats.net_CurrentHP.Value = health;
        stats.Fury.Value = 0;
        stats.Endurance.Value = end;

        float modHealth = stats.GetModifier(StatType.Health);
        stats.RecalculateTotalHealth(modHealth);
    }

    [ServerRpc]
    void LoadCharacterStatsServerRPC(float health, float damage, float attackSpeed, float cdr, float armor, float speed, float fury, float end, float endrech)
    {
        ApplyCharacterStats(health, damage, attackSpeed, cdr, armor, speed, fury, end, endrech);
    }

    void LoadPlayerSkills()
    {
        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        player.FirstPassiveIndex = PlayerPrefs.GetInt($"{slot}FirstPassive", 0);
        player.SecondPassiveIndex = PlayerPrefs.GetInt($"{slot}SecondPassive", -1);
        player.ThirdPassiveIndex = PlayerPrefs.GetInt($"{slot}ThirdPassive", -1);
        player.BasicIndex = PlayerPrefs.GetInt($"{slot}Basic", 0);
        player.OffensiveIndex = PlayerPrefs.GetInt($"{slot}Offensive", -1);
        player.MobilityIndex = PlayerPrefs.GetInt($"{slot}Mobility", -1);
        player.DefensiveIndex = PlayerPrefs.GetInt($"{slot}Defensive", -1);
        player.UtilityIndex = PlayerPrefs.GetInt($"{slot}Utility", -1);
        player.UltimateIndex = PlayerPrefs.GetInt($"{slot}Ultimate", -1);
    }

    public void SaveStats()
    {
        if (!IsOwner) return;
        if (!statsInitialized) return;

        int slot = PlayerPrefs.GetInt("SelectedCharacter");

        PlayerPrefs.SetInt($"{slot}PlayerLevel", stats.PlayerLevel.Value);
        PlayerPrefs.SetInt($"{slot}PlayerClass", (int)stats.playerClass);
        PlayerPrefs.SetFloat($"{slot}CurrentExperience", stats.CurrentExperience.Value);
        PlayerPrefs.SetFloat($"{slot}RequiredExperience", stats.RequiredExperience.Value);
        PlayerPrefs.SetFloat($"{slot}Coins", stats.Coins);
        PlayerPrefs.SetInt($"{slot}AP", stats.AttributePoints.Value);

        // Stats
        PlayerPrefs.SetFloat($"{slot}MaxHealth", stats.net_BaseHP.Value);
        PlayerPrefs.SetFloat($"{slot}MaxFury", stats.MaxFury.Value);
        PlayerPrefs.SetFloat($"{slot}MaxEndurance", stats.MaxEndurance.Value);
        PlayerPrefs.SetFloat($"{slot}EnduranceRecharge", stats.EnduranceRechargeRate.Value);

        PlayerPrefs.SetFloat($"{slot}Speed", stats.net_BaseSpeed.Value);
        PlayerPrefs.SetFloat($"{slot}Damage", stats.net_BaseDamage.Value);
        PlayerPrefs.SetFloat($"{slot}AttackSpeed", stats.net_BaseAS.Value);
        PlayerPrefs.SetFloat($"{slot}CDR", stats.net_BaseCDR.Value);
        PlayerPrefs.SetFloat($"{slot}Armor", stats.net_BaseArmor.Value);

        // Skills
        PlayerPrefs.SetInt($"{slot}FirstPassive", player.FirstPassiveIndex);
        PlayerPrefs.SetInt($"{slot}SecondPassive", player.SecondPassiveIndex);
        PlayerPrefs.SetInt($"{slot}ThirdPassive", player.ThirdPassiveIndex);
        PlayerPrefs.SetInt($"{slot}Basic", player.BasicIndex);
        PlayerPrefs.SetInt($"{slot}Offensive", player.OffensiveIndex);
        PlayerPrefs.SetInt($"{slot}Mobility", player.MobilityIndex);
        PlayerPrefs.SetInt($"{slot}Defensive", player.DefensiveIndex);
        PlayerPrefs.SetInt($"{slot}Utility", player.UtilityIndex);
        PlayerPrefs.SetInt($"{slot}Ultimate", player.UltimateIndex);

        PlayerPrefs.Save();
        StartCoroutine(SaveText());
    }

    public void SaveInventory(InventorySlotData slotData, int slotIndex, bool saveImmediately = true)
    {
        string prefix = $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";
        string key = $"{prefix}InventorySlot_{slotIndex}";

        if (slotData == null || slotData.item == null || slotData.quantity <= 0)
        {
            PlayerPrefs.DeleteKey(key);
            if (saveImmediately) PlayerPrefs.Save();
            return;
        }

        string baseName = slotData.item.name.Replace("(Clone)", "").Trim();
        int qty = slotData.quantity;
        ItemRarity rarity = slotData.rarity;
        ItemQuality quality = slotData.quality;
        List<StatModifier> modifiers = slotData.modifiers ?? new List<StatModifier>();

        // Serialize modifiers as: value,statTypeInt,sourceInt;value,...
        StringBuilder modsSb = new StringBuilder();
        for (int i = 0; i < modifiers.Count; i++)
        {
            StatModifier m = modifiers[i];
            modsSb.Append(m.value)
                  .Append(',')
                  .Append((int)m.statType)
                  .Append(',')
                  .Append((int)m.source);
            if (i < modifiers.Count - 1) modsSb.Append(';');
        }

        string value = $"{baseName}|{qty}|{rarity}|{quality}|{modsSb}";
        PlayerPrefs.SetString(key, value);
        if (saveImmediately) PlayerPrefs.Save();
    }

    public void SaveEquipment(Item item, int slotIndex)
    {
        // Save the equipment slot for the currently selected character
        string prefix = $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";
        string key = $"{prefix}EquipmentSlot_{slotIndex}";

        // If the item is null, delete the key to clear the slot
        if (item == null)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            return;
        }

        // Save the item name in the format "ItemName"
        string baseName = item.name.Replace("(Clone)", "").Trim();

        // Save the value to PlayerPrefs
        PlayerPrefs.SetString(key, baseName);

        // Save immediately
        PlayerPrefs.Save();
    }

    public void LoadInventory()
    {
        // prefix for the PlayerPrefs keys based on the selected character
        string prefix = $"Character{PlayerPrefs.GetInt("SelectedCharacter")}_";

        // for each inventory slot, check if there's a saved item and load it
        for (int i = 0; i < inventory.inventorySlots; i++)
        {
            // Construct the PlayerPrefs key for this slot
            string key = $"{prefix}InventorySlot_{i}";

            // Check if there's a saved item for this slot
            if (PlayerPrefs.HasKey(key))
            {
                // Retrieve the saved string for this slot
                string saved = PlayerPrefs.GetString(key);

                // Split the saved string into parts: item name and quantity
                string[] parts = saved.Split('|');

                // Validate the parts and parse the quantity
                if (parts.Length >= 2 && !string.IsNullOrWhiteSpace(parts[0]) && int.TryParse(parts[1], out int quantity))
                {
                    // Retrieve the item name from the saved string
                    string itemName = parts[0];

                    // Get the item template from the item database using the item name
                    Item template = inventory.itemDatabase.GetItemByName(itemName);

                    // If the template is found, create a new InventorySlotData for this slot
                    if (template != null)
                    {
                        // Create a new InventorySlotData for this slot using the template and saved quantity
                        ItemRarity rarity = template.ItemRarity;
                        ItemQuality quality = template.ItemQuality;
                        List<StatModifier> modifiers = new List<StatModifier>();

                        // If the saved string contains rarity and quality, parse them
                        if (parts.Length >= 4)
                        {
                            // parse enums by name or numeric fallback
                            if (!Enum.TryParse(parts[2], out rarity))
                            {
                                if (int.TryParse(parts[2], out int rInt)) rarity = (ItemRarity)rInt;
                            }
                            if (!Enum.TryParse(parts[3], out quality))
                            {
                                if (int.TryParse(parts[3], out int qInt)) quality = (ItemQuality)qInt;
                            }
                        }

                        // If the saved string contains modifiers, parse them
                        if (parts.Length >= 5 && !string.IsNullOrEmpty(parts[4]))
                        {
                            string modsPart = parts[4];
                            string[] modEntries = modsPart.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            // Parse each modifier entry and create StatModifier instances
                            foreach (string me in modEntries)
                            {
                                string[] modParts = me.Split(',');
                                if (modParts.Length == 3 &&
                                    float.TryParse(modParts[0], out float val) &&
                                    int.TryParse(modParts[1], out int statInt) &&
                                    int.TryParse(modParts[2], out int srcInt))
                                {
                                    StatModifier m = new StatModifier
                                    {
                                        value = val,
                                        statType = (StatType)statInt,
                                        source = (ModSource)srcInt
                                    };
                                    modifiers.Add(m);
                                }
                            }
                        }

                        inventory.items[i] = new InventorySlotData(template, quantity, rarity, quality, modifiers);
                    }
                    else
                    {
                        // If the template is not found, log a warning and clear the slot
                        Debug.LogWarning($"Item '{itemName}' not found in ItemDatabase.");

                        // Force clear the slot if the item is invalid
                        inventory.items[i] = null;
                    }
                }
                else
                {
                    // If the saved string is malformed, log a warning and clear the slot
                    Debug.LogWarning($"Malformed inventory string for key: {key}");

                    // Force clear the slot if the saved string is invalid
                    inventory.items[i] = null;
                }
            }
            else
            {
                // If there's no saved item for this slot, ensure the slot is null
                inventory.items[i] = null;
            }
        }

        // Update the inventory UI after loading all items
        inventory.inventoryUI.UpdateUI();
    }

    IEnumerator SaveText()
    {
        saveText.text = "Save";
        yield return new WaitForSeconds(1);
        saveText.text = "";
    }

    public void SaveQuests()
    {
        if (!IsOwner) return;
        if (playerQuest == null) return;

        int slot = PlayerPrefs.GetInt("SelectedCharacter");
        string prefix = $"Character{slot}_";

        foreach (QuestProgress progress in playerQuest.activeQuests)
        {
            // Only save quests that are in progress or completed, not failed or turned in
            if (progress == null || progress.quest == null) continue;

            //Base key for this quest
            string questKeyBase = $"{prefix}{progress.quest.QuestID}";

            // Save the quest state (InProgress, Completed, etc.)
            PlayerPrefs.SetInt($"{questKeyBase}_State", (int)progress.state);

            foreach (QuestObjective obj in progress.objectives)
            {
                // Save current amount for each objective, using a key that includes the objective ID
                if (obj == null) continue;
                PlayerPrefs.SetInt($"{questKeyBase}_Obj_{obj.ObjectiveID}", obj.CurrentAmount);
            }
        }

        PlayerPrefs.Save();
    }

    void SaveQuestsOnTurnIn(Quest q)
    {
        SaveQuests();
    }

    void LoadQuests()
    {
        if (!IsOwner) return;
        if (playerQuest == null) return;
        if (questList.QuestDatabase == null || questList.QuestDatabase.Length == 0) return;

        int slot = PlayerPrefs.GetInt("SelectedCharacter");
        string prefix = $"Character{slot}_";

        foreach (Quest quest in questList.QuestDatabase)
        {
            if (quest == null) continue;

            string questKeyBase = $"{prefix}{quest.QuestID}";
            string stateKey = $"{questKeyBase}_State";

            // If there's no saved state for this quest, skip it (means it wasn't accepted or progressed yet)
            if (!PlayerPrefs.HasKey(stateKey)) continue;

            // Get saved state, default to InProgress if somehow missing (shouldn't happen since we check HasKey)
            int savedStateInt = PlayerPrefs.GetInt(stateKey, (int)QuestState.InProgress);
            QuestState savedState = (QuestState)savedStateInt;

            // If quest not yet accepted, accept it to reconstruct progress
            QuestProgress existing = playerQuest.activeQuests.Find(qp => qp.quest == quest);
            if (existing == null) playerQuest.AcceptQuest(quest, false);

            // Now update progress values from saved data
            QuestProgress progress = playerQuest.activeQuests.Find(qp => qp.quest == quest);
            if (progress == null) continue;

            // Restore each objective current amount
            foreach (QuestObjective obj in progress.objectives)
            {
                string objKey = $"{questKeyBase}_Obj_{obj.ObjectiveID}";
                if (PlayerPrefs.HasKey(objKey))
                {
                    int savedAmount = PlayerPrefs.GetInt(objKey, 0);
                    obj.CurrentAmount = Mathf.Min(savedAmount, obj.RequiredAmount);
                }
            }

            // Restore saved state
            progress.state = savedState;

            // Ensure completion flags are consistent
            progress.CheckCompletion();
        }

        // Notify listeners so UI and other systems update
        playerQuest.OnQuestStateChanged?.Invoke();
    }
}
