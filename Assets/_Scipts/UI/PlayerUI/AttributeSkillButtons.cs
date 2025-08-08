using UnityEngine;

public class AttributeSkillButtons : MonoBehaviour
{
    [SerializeField] GameObject attribute_Button;
    [SerializeField] GameObject skill_Button;
    [SerializeField] Player player;

    private void Update()
    {
        HandleAttributes();
        HandleSkills();
    }

    void HandleAttributes()
    {
        if (player.AttributePoints.Value == 0)
        {
            attribute_Button.SetActive(false);
        }
        else
        {
            attribute_Button.SetActive(true);
        }
    }

    void HandleSkills()
    {
        if (player.FirstPassiveIndex == -1)
        {
            if (player.PlayerLevel.Value < 1)
            {
                skill_Button.SetActive(true);
            }
        }
    }
}
