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
            nameText.text = npc.NPC_Name;
            levelText.text = "Lvl: " + npc.NPC_Level.ToString();
        }

        if (enemy != null)
        {
            nameText.text = enemy.Enemy_Name;
            levelText.text = "Lvl: " + enemy.Enemy_Level.ToString();
        }
    }
}
