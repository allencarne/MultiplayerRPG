using UnityEngine;

public class SetSkillPanel : MonoBehaviour
{
    [SerializeField] Player player;

    [SerializeField] GameObject BeginnerPanel;
    [SerializeField] GameObject WarriorPanel;
    [SerializeField] GameObject MagicianPanel;
    [SerializeField] GameObject ArcherPanel;
    [SerializeField] GameObject RoguePanel;

    private void Start()
    {
        BeginnerPanel.SetActive(false);
        WarriorPanel.SetActive(false);
        MagicianPanel.SetActive(false);
        ArcherPanel.SetActive(false);
        RoguePanel.SetActive(false);

        GetPlayerClass();
    }

    void GetPlayerClass()
    {
        switch (player.playerClass)
        {
            case Player.PlayerClass.Beginner:
                BeginnerPanel.SetActive(true);
                break;
            case Player.PlayerClass.Warrior:
                WarriorPanel.SetActive(true);
                break;
            case Player.PlayerClass.Magician:
                MagicianPanel.SetActive(true);
                break;
            case Player.PlayerClass.Archer:
                ArcherPanel.SetActive(true);
                break;
            case Player.PlayerClass.Rogue:
                RoguePanel.SetActive(true);
                break;
        }
    }
}
