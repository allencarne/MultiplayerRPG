using System.Collections;
using TMPro;
using UnityEngine;

public class SkillBarCoolDowns : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI basicText;
    [SerializeField] TextMeshProUGUI offensiveText;
    [SerializeField] TextMeshProUGUI mobilityText;
    [SerializeField] TextMeshProUGUI defensiveText;
    [SerializeField] TextMeshProUGUI utilityText;
    [SerializeField] TextMeshProUGUI ultimateText;

    public void SkillCoolDown(PlayerStateMachine.SkillType type, float CoolDown)
    {
        switch (type)
        {
            case PlayerStateMachine.SkillType.Basic: StartCoroutine(TrackSkill(CoolDown, basicText)); break;
            case PlayerStateMachine.SkillType.Offensive: StartCoroutine(TrackSkill(CoolDown, offensiveText)); break;
            case PlayerStateMachine.SkillType.Mobility: StartCoroutine(TrackSkill(CoolDown, mobilityText)); break;
            case PlayerStateMachine.SkillType.Defensive: StartCoroutine(TrackSkill(CoolDown, defensiveText)); break;
            case PlayerStateMachine.SkillType.Utility: StartCoroutine(TrackSkill(CoolDown, utilityText)); break;
            case PlayerStateMachine.SkillType.Ultimate: StartCoroutine(TrackSkill(CoolDown, ultimateText)); break;
        }
    }

    IEnumerator TrackSkill(float cooldown, TextMeshProUGUI text)
    {
        float timeRemaining = cooldown;

        while (timeRemaining > 0f)
        {
            text.text = timeRemaining.ToString("F1"); // Format to 1 decimal place, like 99.9
            yield return null; // Wait for next frame
            timeRemaining -= Time.deltaTime;
        }

        text.text = ""; // Clear the text when cooldown is done
    }
}
