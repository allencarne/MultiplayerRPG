using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillBarCoolDowns : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI basicText;
    [SerializeField] TextMeshProUGUI offensiveText;
    [SerializeField] TextMeshProUGUI mobilityText;
    [SerializeField] TextMeshProUGUI defensiveText;
    [SerializeField] TextMeshProUGUI utilityText;
    [SerializeField] TextMeshProUGUI ultimateText;

    [SerializeField] Image basicImage;
    [SerializeField] Image offensiveImage;
    [SerializeField] Image mobilityImage;
    [SerializeField] Image defensiveImage;
    [SerializeField] Image utilityImage;
    [SerializeField] Image ultimateImage;

    public void SkillCoolDown(PlayerStateMachine.SkillType type, float CoolDown)
    {
        switch (type)
        {
            case PlayerStateMachine.SkillType.Basic: StartCoroutine(TrackSkill(CoolDown, basicText, type)); basicImage.enabled = true; break;
            case PlayerStateMachine.SkillType.Offensive: StartCoroutine(TrackSkill(CoolDown, offensiveText, type)); offensiveImage.enabled = true; break;
            case PlayerStateMachine.SkillType.Mobility: StartCoroutine(TrackSkill(CoolDown, mobilityText, type)); mobilityImage.enabled = true; break;
            case PlayerStateMachine.SkillType.Defensive: StartCoroutine(TrackSkill(CoolDown, defensiveText, type)); defensiveImage.enabled = true; break;
            case PlayerStateMachine.SkillType.Utility: StartCoroutine(TrackSkill(CoolDown, utilityText, type)); utilityImage.enabled = true; break;
            case PlayerStateMachine.SkillType.Ultimate: StartCoroutine(TrackSkill(CoolDown, ultimateText, type)); ultimateImage.enabled = true; break;
        }
    }

    IEnumerator TrackSkill(float cooldown, TextMeshProUGUI text, PlayerStateMachine.SkillType type)
    {
        float timeRemaining = cooldown;

        while (timeRemaining > 0f)
        {
            text.text = timeRemaining.ToString("F1");
            yield return null;
            timeRemaining -= Time.deltaTime;
        }

        text.text = "";

        switch (type)
        {
            case PlayerStateMachine.SkillType.Basic: basicImage.enabled = false; break;
            case PlayerStateMachine.SkillType.Offensive: offensiveImage.enabled = false; break;
            case PlayerStateMachine.SkillType.Mobility: mobilityImage.enabled = false; break;
            case PlayerStateMachine.SkillType.Defensive: defensiveImage.enabled = false; break;
            case PlayerStateMachine.SkillType.Utility: utilityImage.enabled = false; break;
            case PlayerStateMachine.SkillType.Ultimate: ultimateImage.enabled = false; break;
        }
    }
}
