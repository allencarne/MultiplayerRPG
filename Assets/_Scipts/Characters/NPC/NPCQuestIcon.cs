using UnityEngine;
using UnityEngine.UI;

public class NPCQuestIcon : MonoBehaviour
{
    [SerializeField] Image questIcon;
    [SerializeField] Sprite[] icons;
    NPCQuest questGiver;

    public void RefreshIcon(Player player)
    {
        if (questGiver == null) questGiver = GetComponent<NPCQuest>();
        if (questGiver == null) return;

        //QuestState state = questGiver.GetQuestStateForPlayer(player);
        //questIcon.sprite = icons[(int)state];
    }
}
