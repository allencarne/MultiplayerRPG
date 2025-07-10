using System;
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

    public void SkillCoolDown(PlayerStateMachine.SkillType type, float baseCooldown, Func<float> getAdjustedDuration, Func<float> getElapsedTime)
    {
        switch (type)
        {
            case PlayerStateMachine.SkillType.Basic: StartCoroutine(TrackSkill(basicText, getAdjustedDuration, getElapsedTime)); break;
            case PlayerStateMachine.SkillType.Offensive: StartCoroutine(TrackSkill(offensiveText, getAdjustedDuration, getElapsedTime)); break;
            case PlayerStateMachine.SkillType.Mobility: StartCoroutine(TrackSkill(mobilityText, getAdjustedDuration, getElapsedTime)); break;
            case PlayerStateMachine.SkillType.Defensive: StartCoroutine(TrackSkill(defensiveText, getAdjustedDuration, getElapsedTime)); break;
            case PlayerStateMachine.SkillType.Utility: StartCoroutine(TrackSkill(utilityText, getAdjustedDuration, getElapsedTime)); break;
            case PlayerStateMachine.SkillType.Ultimate: StartCoroutine(TrackSkill(ultimateText, getAdjustedDuration, getElapsedTime)); break;
        }
    }

    IEnumerator TrackSkill(TextMeshProUGUI text, Func<float> getDuration, Func<float> getElapsedTime)
    {
        while (true)
        {
            float duration = getDuration();
            float elapsed = getElapsedTime();
            float remaining = Mathf.Clamp(duration - elapsed, 0f, duration);

            if (remaining <= 0f)
                break;

            text.text = remaining.ToString("F1");
            yield return null;
        }

        text.text = "";
    }
}
