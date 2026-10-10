using TMPro;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] NPC npc;
    [SerializeField] Enemy enemy;

    private void Start()
    {
        UpdateStatusBar();
    }

    public void UpdateStatusBar()
    {
        if (npc != null)
        {
            switch (npc.Data.npcClass)
            {
                case NPCClass.Quest:
                    nameText.text = $"<sprite name=\"Spr_Icon_Class_NPC_5\"> {npc.Data.NPCName}";
                    break;
                case NPCClass.Vendor:
                    nameText.text = $"<sprite name=\"Spr_Icon_Class_NPC_2\"> {npc.Data.NPCName}";
                    break;
                case NPCClass.Guard:
                    nameText.text = $"<sprite name=\"Spr_Icon_Class_NPC_0\"> {npc.Data.NPCName}";
                    break;
                case NPCClass.Patrol:
                    nameText.text = $"<sprite name=\"Spr_Icon_Class_NPC_1\"> {npc.Data.NPCName}";
                    break;
                case NPCClass.Villager:
                    nameText.text = $"<sprite name=\"Spr_Icon_Class_NPC_4\"> {npc.Data.NPCName}";
                    break;
                case NPCClass.Refiner:
                    nameText.text = $"<sprite name=\"Spr_Icon_Class_NPC_3\"> {npc.Data.NPCName}";
                    break;
            }

            levelText.text = "Lvl: " + npc.Data.NPC_Level.ToString();
        }

        if (enemy != null)
        {
            nameText.text = enemy.Data.Enemy_Name;
            levelText.text = "Lvl: " + enemy.Data.Enemy_Level.ToString();
        }
    }
}
