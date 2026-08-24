using UnityEngine;

public class ClassSkillSelector : MonoBehaviour
{
    [Header("Refences")]
    [SerializeField] PlayerStats stats;
    [SerializeField] Player player;
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] SkillPanelUI skillPanel;

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
    }

    private void OnDisable()
    {
        skillPanel.OnSkillSelected.RemoveListener(SetClassSkills);
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
    }
}
