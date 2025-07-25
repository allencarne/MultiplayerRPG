using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour
{
    [SerializeField] Button AcceptButton;
    [SerializeField] Button DeclineButton;

    public Quest[] Quests;
    [SerializeField] GameObject QuestUI;
    Player playerReference;

    public void StartQuest(Player player)
    {
        QuestUI.SetActive(true);
        //UpdateDialogueText();
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(AcceptButton.gameObject);
        if (playerReference == null) playerReference = player;
    }
}
