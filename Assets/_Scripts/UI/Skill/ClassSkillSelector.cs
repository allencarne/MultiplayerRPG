using System;
using UnityEngine;

public class ClassSkillSelector : MonoBehaviour
{
    [Header("Refences")]
    [SerializeField] PlayerStats stats;
    [SerializeField] Player player;
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] SkillPanelUI skillPanel;
    [SerializeField] SkillBarUI[] skillBarUI;

    [Header("Skills")]
    public ClassSkillSet beginnerSet;
    public ClassSkillSet warriorSet;
    public ClassSkillSet magicianSet;
    public ClassSkillSet archerSet;
    public ClassSkillSet rogueSet;
    ClassSkillSet currentSet;

    private void OnEnable()
    {
        skillPanel.OnSkillSelected.AddListener(SetClassSkills);
        skillPanel.OnSkillSelected.AddListener(RefreshSkillBars);
    }

    private void OnDisable()
    {
        skillPanel.OnSkillSelected.RemoveListener(SetClassSkills);
        skillPanel.OnSkillSelected.RemoveListener(RefreshSkillBars);
    }

    private void Awake()
    {
        SetClassSkills();
    }

    public void SetClassSkills()
    {
        currentSet = stats.playerClass switch
        {
            PlayerStats.PlayerClass.Beginner => beginnerSet,
            PlayerStats.PlayerClass.Warrior => warriorSet,
            PlayerStats.PlayerClass.Magician => magicianSet,
            PlayerStats.PlayerClass.Archer => archerSet,
            PlayerStats.PlayerClass.Rogue => rogueSet,
            _ => null
        };

        stateMachine.skills = currentSet;
        skillPanel.Bind(currentSet);
        foreach (SkillBarUI bar in skillBarUI) bar.Bind(currentSet);
    }

    void RefreshSkillBars()
    {
        foreach (SkillBarUI bar in skillBarUI) bar.RefreshIcons();
    }

    public void RestoreSelections()
    {
        if (currentSet == null) return;

        RestoreSlot(player.FirstPassiveIndex, currentSet.passive1Req, currentSet.firstPassive.Length, skillPanel.FirstPassiveButton);
        RestoreSlot(player.SecondPassiveIndex, currentSet.passive2Req, currentSet.secondPassive.Length, skillPanel.SecondPassiveButton);
        RestoreSlot(player.ThirdPassiveIndex, currentSet.passive3Req, currentSet.thirdPassive.Length, skillPanel.ThirdPassiveButton);
        RestoreSlot(player.BasicIndex, currentSet.basicReq, currentSet.basicAbilities.Length, skillPanel.BasicButton);
        RestoreSlot(player.OffensiveIndex, currentSet.offensiveReq, currentSet.offensiveAbilities.Length, skillPanel.OffensiveButton);
        RestoreSlot(player.MobilityIndex, currentSet.mobilityReq, currentSet.mobilityAbilities.Length, skillPanel.MobilityButton);
        RestoreSlot(player.DefensiveIndex, currentSet.defensiveReq, currentSet.defensiveAbilities.Length, skillPanel.DefensiveButton);
        RestoreSlot(player.UtilityIndex, currentSet.utilityReq, currentSet.utilityAbilities.Length, skillPanel.UtilityButton);
        RestoreSlot(player.UltimateIndex, currentSet.ultimateReq, currentSet.ultimateAbilities.Length, skillPanel.UltimateButton);
    }

    void RestoreSlot(int index, int reqLevel, int arrayLength, Action<int> select)
    {
        if (index < 0 || index >= arrayLength) return;
        if (stats.PlayerLevel.Value < reqLevel) return;
        select(index);
    }
}
