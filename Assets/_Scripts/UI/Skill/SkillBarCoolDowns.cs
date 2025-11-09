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

    [SerializeField] TextMeshProUGUI basicText_m;
    [SerializeField] TextMeshProUGUI offensiveText_m;
    [SerializeField] TextMeshProUGUI mobilityText_m;
    [SerializeField] TextMeshProUGUI defensiveText_m;
    [SerializeField] TextMeshProUGUI utilityText_m;
    [SerializeField] TextMeshProUGUI ultimateText_m;

    [SerializeField] Image basicImage;
    [SerializeField] Image offensiveImage;
    [SerializeField] Image mobilityImage;
    [SerializeField] Image defensiveImage;
    [SerializeField] Image utilityImage;
    [SerializeField] Image ultimateImage;

    [SerializeField] Image basicImage_m;
    [SerializeField] Image offensiveImage_m;
    [SerializeField] Image mobilityImage_m;
    [SerializeField] Image defensiveImage_m;
    [SerializeField] Image utilityImage_m;
    [SerializeField] Image ultimateImage_m;

    public void SkillCoolDown(PlayerSkill.SkillType type, float CoolDown)
    {
        switch (type)
        {
            case PlayerSkill.SkillType.Basic: StartCoroutine(TrackSkill(CoolDown, basicText, basicText_m, basicImage , basicImage_m)); break;
            case PlayerSkill.SkillType.Offensive: StartCoroutine(TrackSkill(CoolDown, offensiveText, offensiveText_m, offensiveImage, offensiveImage_m));  break;
            case PlayerSkill.SkillType.Mobility: StartCoroutine(TrackSkill(CoolDown, mobilityText, mobilityText_m, mobilityImage, mobilityImage_m)); break;
            case PlayerSkill.SkillType.Defensive: StartCoroutine(TrackSkill(CoolDown, defensiveText, defensiveText_m, defensiveImage, defensiveImage_m)); break;
            case PlayerSkill.SkillType.Utility: StartCoroutine(TrackSkill(CoolDown, utilityText, utilityText_m, utilityImage, utilityImage_m)); break;
            case PlayerSkill.SkillType.Ultimate: StartCoroutine(TrackSkill(CoolDown, ultimateText, ultimateText_m, ultimateImage, ultimateImage_m)); break;
        }
    }

    IEnumerator TrackSkill(float cooldown, TextMeshProUGUI text, TextMeshProUGUI mText, Image image, Image mImage)
    {
        image.enabled = true;
        mImage.enabled = true;

        float timeRemaining = cooldown;

        while (timeRemaining > 0f)
        {
            text.text = timeRemaining.ToString("F1");
            mText.text = timeRemaining.ToString("F1");
            yield return null;
            timeRemaining -= Time.deltaTime;
        }

        text.text = "";
        mText.text = "";

        image.enabled = false;
        mImage.enabled = false;
    }
}
