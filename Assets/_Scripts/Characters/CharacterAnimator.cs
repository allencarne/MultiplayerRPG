using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [Header("Animators")]
    public Animator HeadAnimator;
    public Animator BodyAnimator;
    public Animator ChestAnimator;
    public Animator LegsAnimator;
    public Animator WeaponAnimator;

    [Header("Primary Animator")]
    public Animator PrimaryAnimator;

    public void PlayAnimation(string stateName, int chestIndex, int legIndex, WeaponType weaponType)
    {
        if (HeadAnimator) HeadAnimator.Play(stateName, -1, 0);
        if (BodyAnimator) BodyAnimator.Play(stateName, -1, 0);

        if (ChestAnimator)
        {
            string chestState = $"{stateName}_{chestIndex}";
            ChestAnimator.Play(chestState, -1, 0);
        }

        if (LegsAnimator)
        {
            string legsState = $"{stateName}_{legIndex}";
            LegsAnimator.Play(legsState, -1, 0);
        }

        if (WeaponAnimator)
        {
            string weaponState = $"{weaponType} {stateName}";
            WeaponAnimator.Play(weaponState, -1, 0);
        }
    }

    public void PlayEnemyAnimation(string stateName)
    {
        if (PrimaryAnimator) PrimaryAnimator.Play(stateName, -1, 0);
    }

    public void PlayAttackAnimation(WeaponType weapon, ActiveSkillData.SkillType type, ActiveSkillData.SkillPhase state, int chestIndex, int legIndex)
    {
        string _weapon = "";
        string _skill = "";
        string _state = "";

        switch (weapon)
        {
            case WeaponType.Sword: _weapon = weapon.ToString(); break;
            case WeaponType.Staff: _weapon = weapon.ToString(); break;
            case WeaponType.Bow: _weapon = weapon.ToString(); break;
            case WeaponType.Dagger: _weapon = weapon.ToString(); break;
        }

        switch (type)
        {
            case ActiveSkillData.SkillType.Basic: _skill = "Basic"; break;
            case ActiveSkillData.SkillType.Offensive: _skill = "Offensive"; break;
            case ActiveSkillData.SkillType.Mobility: _skill = "Mobility"; break;
            case ActiveSkillData.SkillType.Defensive: _skill = "Defensive"; break;
            case ActiveSkillData.SkillType.Utility: _skill = "Utility"; break;
            case ActiveSkillData.SkillType.Ultimate: _skill = "Ultimate"; break;
        }

        switch (state)
        {
            case ActiveSkillData.SkillPhase.Cast: _state = "Cast"; break;
            case ActiveSkillData.SkillPhase.Action: _state = "Action"; break;
            case ActiveSkillData.SkillPhase.Impact: _state = "Impact"; break;
            case ActiveSkillData.SkillPhase.Recovery: _state = "Recovery"; break;
            case ActiveSkillData.SkillPhase.Done: _state = "Done"; break;
        }

        HeadAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state);
        BodyAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state);
        ChestAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state + " " + chestIndex);
        LegsAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state + " " + legIndex);
        WeaponAnimator.Play(_weapon + " " + _skill + " " + "Front" + " " + _state);
    }

    public void PlayEnemyAttackAnimation(ActiveSkillData.SkillType type, EnemySkill.State state)
    {
        string animationType = "";
        string animationState = "";

        switch (type)
        {
            case ActiveSkillData.SkillType.Basic: animationType = "Basic"; break;
            case ActiveSkillData.SkillType.Mobility: animationType = "Special"; break;
            case ActiveSkillData.SkillType.Ultimate: animationType = "Ultimate"; break;
        }

        switch (state)
        {
            case EnemySkill.State.Cast: animationState = "Cast"; break;
            case EnemySkill.State.Action: animationState = "Action"; break;
            case EnemySkill.State.Impact: animationState = "Impact"; break;
            case EnemySkill.State.Recovery: animationState = "Recovery"; break;
            case EnemySkill.State.Done: animationState = "Done"; break;
        }

        PrimaryAnimator.Play(animationType + " " + animationState);
    }

    public void PlayStaggerAnimation()
    {
        // For Players and NPC's only

        // Stop all animations
        HeadAnimator.speed = 0;
        BodyAnimator.speed = 0;
        ChestAnimator.speed = 0;
        LegsAnimator.speed = 0;
        WeaponAnimator.speed = 0;

        // Player Spawn Animation to Hide Clothes
        ChestAnimator.Play("Spawn");
        LegsAnimator.Play("Spawn");
    }

    public void EndStaggerAnimation()
    {
        // For Players and NPC's only

        // Stop all animations
        HeadAnimator.speed = 1;
        BodyAnimator.speed = 1;
        ChestAnimator.speed = 1;
        LegsAnimator.speed = 1;
        WeaponAnimator.speed = 1;
    }

    public void SetDirection(Vector2 direction)
    {
        if (HeadAnimator)
        {
            HeadAnimator.SetFloat("Horizontal", direction.x);
            HeadAnimator.SetFloat("Vertical", direction.y);
        }
        if (BodyAnimator)
        {
            BodyAnimator.SetFloat("Horizontal", direction.x);
            BodyAnimator.SetFloat("Vertical", direction.y);
        }
        if (ChestAnimator)
        {
            ChestAnimator.SetFloat("Horizontal", direction.x);
            ChestAnimator.SetFloat("Vertical", direction.y);
        }
        if (LegsAnimator)
        {
            LegsAnimator.SetFloat("Horizontal", direction.x);
            LegsAnimator.SetFloat("Vertical", direction.y);
        }
        if (WeaponAnimator)
        {
            WeaponAnimator.SetFloat("Horizontal", direction.x);
            WeaponAnimator.SetFloat("Vertical", direction.y);
        }
    }

    public Vector2 SnapDirection(Vector2 direction)
    {
        // This Code allows the Last Input direction to be animated

        // Check if the x component of the direction is greater in magnitude than the y component
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // Snap to the horizontal axis by setting the y component to 0
            direction.y = 0;

            // Normalize the x component to either 1 or -1 depending on its original sign
            direction.x = Mathf.Sign(direction.x);
        }
        else
        {
            // Snap to the vertical axis by setting the x component to 0
            direction.x = 0;

            // Normalize the y component to either 1 or -1 depending on its original sign
            direction.y = Mathf.Sign(direction.y);
        }

        // Return the modified direction vector, now snapped to either horizontal or vertical
        return direction;
    }

    public Vector2 GetAnimationDirection(Vector2 input, bool isGamepad)
    {
        if (isGamepad)
        {
            // Standard 4-direction: whichever axis is dominant wins
            // If the absolute value of the x component is greater than or equal to the absolute value of the y component, snap to horizontal
            if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            {
                // Snap to horizontal by returning a vector with the sign of the x component and 0 for the y component
                return new Vector2(Mathf.Sign(input.x), 0);
            }
            else
            {
                // Snap to vertical by returning a vector with 0 for the x component and the sign of the y component
                return new Vector2(0, Mathf.Sign(input.y));
            }
        }
        else
        {
            // Keyboard: horizontal takes priority when both axes are held
            // If the x component is not zero, snap to horizontal; otherwise, snap to vertical
            if (input.x != 0)
            {
                // Snap to horizontal by returning a vector with the sign of the x component and 0 for the y component
                return new Vector2(Mathf.Sign(input.x), 0);
            }
            else
            {
                // Snap to vertical by returning a vector with 0 for the x component and the sign of the y component
                return new Vector2(0, Mathf.Sign(input.y));
            }
        }
    }

    public bool UsingGamepad(PlayerStateMachine owner)
    {
        return owner.playerInput != null && owner.playerInput.currentControlScheme == "Gamepad";
    }
}
