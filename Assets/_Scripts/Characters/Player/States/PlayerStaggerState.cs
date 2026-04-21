
public class PlayerStaggerState : PlayerState
{
    public override void StartState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;

        owner.PlayerHeadAnimator.Play("Stagger", -1, 0);
        owner.BodyAnimator.Play("Stagger", -1, 0);

        // Hide Clothes, Eyes, & Hair?
        //owner.ChestAnimator.Play("Stagger_" + owner.customization.net_ChestIndex.Value, -1, 0);
        //owner.LegsAnimator.Play("Stagger_" + owner.customization.net_LegsIndex.Value, -1, 0);

        if (owner.Equipment.IsWeaponEquipped)
        {
            owner.WeaponAnimator.Play(owner.customization.WeaponAnimType + " Stagger", -1, 0);
        }
    }

    public override void UpdateState(PlayerStateMachine owner)
    {
        if (!owner.IsOwner) return;
        if (owner.Stats.isDead) return;

        if (!owner.CrowdControl.knockBack.IsKnockedBack &&
            !owner.CrowdControl.stun.IsStunned &&
            !owner.CrowdControl.knockUp.IsKnockedUp &&
            !owner.CrowdControl.pull.IsPulled)
        {
            owner.SetState(PlayerStateMachine.State.Idle);
        }
    }

    public override void FixedUpdateState(PlayerStateMachine owner)
    {

    }
}
