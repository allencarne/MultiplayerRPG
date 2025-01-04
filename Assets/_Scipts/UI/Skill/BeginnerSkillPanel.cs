using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.UI;

public class BeginnerSkillPanel : MonoBehaviour
{
    [SerializeField] PlayerAbilities playerAbilities;

    [SerializeField] ScriptableObject basicAbility;
    [SerializeField] Image basicAbilityImage;
    private void Start()
    {
        Sprite icon = (Sprite)basicAbility.GetType().GetField("icon").GetValue(basicAbility);

        if (icon != null)
        {
            basicAbilityImage.sprite = icon;
        }

        playerAbilities.SetBasicAbility(basicAbility);
    }
}
