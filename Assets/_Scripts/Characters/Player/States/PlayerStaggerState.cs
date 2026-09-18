
using System.Collections;
using UnityEngine;

public class PlayerStaggerState : PlayerState
{
    public PlayerStaggerState(PlayerStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        if (!owner.IsOwner) return;
        owner.Animator.SetStaggerFrozen(true);
    }

    public override void UpdateState()
    {
        // Check if the player is the owner and not dead
        if (!owner.IsOwner) return;
        if (owner.PlayerStats.isDead) return;

        // Check if the player is no longer crowd controlled
        if (!owner.CrowdControl.IsCrowdControlled)
        {
            //owner.Animator.EndStaggerAnimation();
            owner.SetState(new PlayerIdleState(owner));
        }
    }

    public override void ExitState()
    {
        owner.Animator.SetStaggerFrozen(false);
    }
}
