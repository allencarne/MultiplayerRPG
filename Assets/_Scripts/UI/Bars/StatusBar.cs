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
                    nameText.text = $"<sprite name=\"Quest_Icon\"> {npc.Data.NPCName}";
                    break;
                case NPCClass.Vendor:
                    nameText.text = $"<sprite name=\"Vendor_Icon\"> {npc.Data.NPCName}";
                    break;
                case NPCClass.Guard:
                    nameText.text = $"<sprite name=\"Guard_Icon\"> {npc.Data.NPCName}";
                    break;
                case NPCClass.Patrol:
                    nameText.text = $"<sprite name=\"Patrol_Icon\"> {npc.Data.NPCName}";
                    break;
                case NPCClass.Villager:
                    break;
            }

            levelText.text = "Lvl: " + npc.Data.NPC_Level.ToString();
        }

        if (enemy != null)
        {
            nameText.text = enemy.Enemy_Name;
            levelText.text = "Lvl: " + enemy.Enemy_Level.ToString();
        }
    }
}
