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

    public void SkillCoolDown(PlayerStateMachine.SkillType type, float CoolDown, Func<float> getCurrentCDR)
    {
        switch (type)
        {
            case PlayerStateMachine.SkillType.Basic: StartCoroutine(TrackSkill(CoolDown, basicText, getCurrentCDR)); break;
            case PlayerStateMachine.SkillType.Offensive: StartCoroutine(TrackSkill(CoolDown, offensiveText, getCurrentCDR)); break;
            case PlayerStateMachine.SkillType.Mobility: StartCoroutine(TrackSkill(CoolDown, mobilityText, getCurrentCDR)); break;
            case PlayerStateMachine.SkillType.Defensive: StartCoroutine(TrackSkill(CoolDown, defensiveText, getCurrentCDR)); break;
            case PlayerStateMachine.SkillType.Utility: StartCoroutine(TrackSkill(CoolDown, utilityText, getCurrentCDR)); break;
            case PlayerStateMachine.SkillType.Ultimate: StartCoroutine(TrackSkill(CoolDown, ultimateText, getCurrentCDR)); break;
        }
    }

    IEnumerator TrackSkill(float baseCooldown, TextMeshProUGUI text, Func<float> getCurrentCDR)
    {
        float elapsed = 0f;

        while (elapsed < baseCooldown / getCurrentCDR())
        {
            float duration = baseCooldown / getCurrentCDR();
            float timeRemaining = Mathf.Max(0f, duration - elapsed);
            text.text = timeRemaining.ToString("F1");

            elapsed += Time.deltaTime;
            yield return null;
        }

        text.text = "";
    }
}
