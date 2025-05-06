using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject powerUp_Effect;

    [Header("Skills")]
    public PlayerAbility[] firstPassive;
    public PlayerAbility[] basicAbilities;
    public PlayerAbility[] offensiveAbilities;
    public PlayerAbility[] mobilityAbilities;
    public PlayerAbility[] defensiveAbilities;
    public PlayerAbility[] utilityAbilities;
    public PlayerAbility[] ultimateAbilities;

    [Header("FirstPassive")]
    [SerializeField] Image icon_FirstPassive0;
    [SerializeField] Image icon_FirstPassive1;
    [SerializeField] Image icon_FirstPassive2;
    [Header("Basic")]
    [SerializeField] Image icon_Basic0;
    [SerializeField] Image icon_Basic1;
    [SerializeField] Image icon_Basic2;
    [Header("Offensive 1")]
    [SerializeField] Image icon_Offensive0;
    [SerializeField] Image icon_Offensive1;
    [SerializeField] Image icon_Offensive2;
    [Header("Mobility")]
    [SerializeField] Image icon_Mobility0;
    [SerializeField] Image icon_Mobility1;
    [SerializeField] Image icon_Mobility2;
    [Header("Defensive")]
    [SerializeField] Image icon_Defensive0;
    [SerializeField] Image icon_Defensive1;
    [SerializeField] Image icon_Defensive2;
    [Header("Utility")]
    [SerializeField] Image icon_Utility0;
    [SerializeField] Image icon_Utility1;
    [SerializeField] Image icon_Utility2;
    [Header("Ultimate")]
    [SerializeField] Image icon_Ultimate0;
    [SerializeField] Image icon_Ultimate1;
    [SerializeField] Image icon_Ultimate2;

    private void Start()
    {
        AssignIcon(icon_FirstPassive0, firstPassive, 0);
        AssignIcon(icon_FirstPassive1, firstPassive, 1);
        AssignIcon(icon_FirstPassive2, firstPassive, 2);

        AssignIcon(icon_Basic0, basicAbilities, 0);
        AssignIcon(icon_Basic1, basicAbilities, 1);
        AssignIcon(icon_Basic2, basicAbilities, 2);
        AssignIcon(icon_Offensive0, offensiveAbilities, 0);
        AssignIcon(icon_Offensive1, offensiveAbilities, 1);
        AssignIcon(icon_Offensive2, offensiveAbilities, 2);
        AssignIcon(icon_Mobility0, mobilityAbilities, 0);
        AssignIcon(icon_Mobility1, mobilityAbilities, 1);
        AssignIcon(icon_Mobility2, mobilityAbilities, 2);
        AssignIcon(icon_Defensive0, defensiveAbilities, 0);
        AssignIcon(icon_Defensive1, defensiveAbilities, 1);
        AssignIcon(icon_Defensive2, defensiveAbilities, 2);
        AssignIcon(icon_Utility0, utilityAbilities, 0);
        AssignIcon(icon_Utility1, utilityAbilities, 1);
        AssignIcon(icon_Utility2, utilityAbilities, 2);
        AssignIcon(icon_Ultimate0, ultimateAbilities, 0);
        AssignIcon(icon_Ultimate1, ultimateAbilities, 1);
        AssignIcon(icon_Ultimate2, ultimateAbilities, 2);

        YellowBorder(player.FirstPassiveIndex, 0, icon_FirstPassive0);
        YellowBorder(player.FirstPassiveIndex, 0, icon_FirstPassive1);
        YellowBorder(player.FirstPassiveIndex, 0, icon_FirstPassive2);

        YellowBorder(player.BasicIndex,0, icon_Basic0);
        YellowBorder(player.BasicIndex, 0, icon_Basic1);
        YellowBorder(player.BasicIndex, 0, icon_Basic2);
        YellowBorder(player.OffensiveIndex, 4, icon_Offensive0);
        YellowBorder(player.OffensiveIndex, 4, icon_Offensive1);
        YellowBorder(player.OffensiveIndex, 4, icon_Offensive2);
        YellowBorder(player.MobilityIndex, 8, icon_Mobility0);
        YellowBorder(player.MobilityIndex, 8, icon_Mobility1);
        YellowBorder(player.MobilityIndex, 8, icon_Mobility2);
        YellowBorder(player.DefensiveIndex, 12, icon_Defensive0);
        YellowBorder(player.DefensiveIndex, 12, icon_Defensive1);
        YellowBorder(player.DefensiveIndex, 12, icon_Defensive2);
        YellowBorder(player.UtilityIndex, 16, icon_Utility0);
        YellowBorder(player.UtilityIndex, 16, icon_Utility1);
        YellowBorder(player.UtilityIndex, 16, icon_Utility2);
        YellowBorder(player.UltimateIndex, 20, icon_Ultimate0);
        YellowBorder(player.UltimateIndex, 20, icon_Ultimate1);
        YellowBorder(player.UltimateIndex, 20, icon_Ultimate2);
    }

    private void AssignIcon(Image icon, PlayerAbility[] abilities, int index)
    {
        if (icon == null || abilities == null || index >= abilities.Length || abilities[index] == null)
            return;

        if (abilities[index].SkillIcon != null)
            icon.sprite = abilities[index].SkillIcon;
    }

    void YellowBorder(int index, int reqLevel, Image icon)
    {
        if (icon == null) return;
        if (index > -1) return;
        if (player.PlayerLevel < reqLevel) return;

        SetColor(icon, Color.yellow);
    }

    void SetColor(Image icon, Color color)
    {
        Button button = icon.GetComponentInParent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        button.colors = colors;
    }

    public void FirstPassiveButton(int index)
    {
        player.FirstPassiveIndex = index;
        EffectClientRPC();

        switch (index)
        {
            case 0:
                SetColor(icon_FirstPassive0, Color.blue);
                SetColor(icon_FirstPassive1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_FirstPassive2, new Color(1f, 1f, 1f, 0f));
                break;
            case 1:
                SetColor(icon_FirstPassive0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_FirstPassive1, Color.blue);
                SetColor(icon_FirstPassive2, new Color(1f, 1f, 1f, 0f));
                break;
            case 2:
                SetColor(icon_FirstPassive0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_FirstPassive1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_FirstPassive2, Color.blue);
                break;
        }

        PlayerStateMachine stateMachine = player.GetComponent<PlayerStateMachine>();
        firstPassive[index].StartAbility(stateMachine);
    }

    public void BasicButton(int index)
    {
        //if (player.PlayerLevel < 2) return;
        player.BasicIndex = index;
        EffectClientRPC();

        switch (index)
        {
            case 0:
                SetColor(icon_Basic0,Color.blue);
                SetColor(icon_Basic1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Basic2, new Color(1f, 1f, 1f, 0f));
                break;
            case 1:
                SetColor(icon_Basic0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Basic1, Color.blue);
                SetColor(icon_Basic2, new Color(1f, 1f, 1f, 0f));
                break;
            case 2:
                SetColor(icon_Basic0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Basic1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Basic2, Color.blue);
                break;
        }
    }

    public void OffensiveButton(int index)
    {
        if (player.PlayerLevel < 4) return;
        player.OffensiveIndex = index;
        EffectClientRPC();

        switch (index)
        {
            case 0:
                SetColor(icon_Offensive0, Color.blue);
                SetColor(icon_Offensive1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Offensive2, new Color(1f, 1f, 1f, 0f));
                break;
            case 1:
                SetColor(icon_Offensive0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Offensive1, Color.blue);
                SetColor(icon_Offensive2, new Color(1f, 1f, 1f, 0f));
                break;
            case 2:
                SetColor(icon_Offensive0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Offensive1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Offensive2, Color.blue);
                break;
        }
    }

    public void MobilityButton(int index)
    {
        if (player.PlayerLevel < 8) return;
        player.MobilityIndex = index;
        EffectClientRPC();

        switch (index)
        {
            case 0:
                SetColor(icon_Mobility0, Color.blue);
                SetColor(icon_Mobility1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Mobility2, new Color(1f, 1f, 1f, 0f));
                break;
            case 1:
                SetColor(icon_Mobility0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Mobility1, Color.blue);
                SetColor(icon_Mobility2, new Color(1f, 1f, 1f, 0f));
                break;
            case 2:
                SetColor(icon_Mobility0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Mobility1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Mobility2, Color.blue);
                break;
        }
    }

    public void DefensiveButton(int index)
    {
        if (player.PlayerLevel < 12) return;
        player.DefensiveIndex = index;
        EffectClientRPC();

        switch (index)
        {
            case 0:
                SetColor(icon_Defensive0, Color.blue);
                SetColor(icon_Defensive1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Defensive2, new Color(1f, 1f, 1f, 0f));
                break;
            case 1:
                SetColor(icon_Defensive0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Defensive1, Color.blue);
                SetColor(icon_Defensive2, new Color(1f, 1f, 1f, 0f));
                break;
            case 2:
                SetColor(icon_Defensive0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Defensive1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Defensive2, Color.blue);
                break;
        }
    }

    public void UtilityButton(int index)
    {
        if (player.PlayerLevel < 16) return;
        player.UtilityIndex = index;
        EffectClientRPC();

        switch (index)
        {
            case 0:
                SetColor(icon_Utility0, Color.blue);
                SetColor(icon_Utility1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Utility2, new Color(1f, 1f, 1f, 0f));
                break;
            case 1:
                SetColor(icon_Utility0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Utility1, Color.blue);
                SetColor(icon_Utility2, new Color(1f, 1f, 1f, 0f));
                break;
            case 2:
                SetColor(icon_Utility0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Utility1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Utility2, Color.blue);
                break;
        }
    }

    public void UltimateButton(int index)
    {
        if (player.PlayerLevel < 20) return;
        player.UltimateIndex = index;
        EffectClientRPC();

        switch (index)
        {
            case 0:
                SetColor(icon_Ultimate0, Color.blue);
                SetColor(icon_Ultimate1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Ultimate2, new Color(1f, 1f, 1f, 0f));
                break;
            case 1:
                SetColor(icon_Ultimate0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Ultimate1, Color.blue);
                SetColor(icon_Ultimate2, new Color(1f, 1f, 1f, 0f));
                break;
            case 2:
                SetColor(icon_Ultimate0, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Ultimate1, new Color(1f, 1f, 1f, 0f));
                SetColor(icon_Ultimate2, Color.blue);
                break;
        }
    }

    [ClientRpc]
    void EffectClientRPC()
    {
        GameObject effect = Instantiate(powerUp_Effect, player.transform.position, Quaternion.identity);
        Destroy(effect, 1);
    }
}
