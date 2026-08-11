
public class EnemyStaggerState : EnemyState
{
    public EnemyStaggerState(EnemyStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        owner.EnemyAnimator.Play("Stagger");
    }

    public override void UpdateState()
    {
        if (!owner.IsServer) return;
        if (owner.enemy.stats.isDead) return;

        if (!owner.CrowdControl.knockBack.IsKnockedBack &&
            !owner.CrowdControl.stun.IsStunned &&
            !owner.CrowdControl.knockUp.IsKnockedUp &&
            !owner.CrowdControl.pull.IsPulled)
        {
            if (owner.isResetting)
            {
                owner.TransitionToReset();
            }
            else
            {

                owner.TransitionToIdle();
            }
        }
    }
}