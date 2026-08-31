using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        owner.Animator.PlayAnimation("Idle", owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value, owner.customization.WeaponAnimType);
    }

    public override void UpdateState()
    {
        owner.Roll();
        owner.BasicAbility();
        owner.OffensiveAbility();
        owner.MobilityAbility();
        owner.DefensiveAbility();
        owner.UtilityAbility();
        owner.UltimateAbility();
    }

    public override void FixedUpdateState()
    {
        // Transition to Move State
        if (owner.Input.MoveInput != Vector2.zero)
        {
            if (!owner.CrowdControl.immobilize.IsImmobilized)
            {
                owner.SetState(new PlayerRunState(owner));
            }
        }
    }
}
