
public class EnemyStaggerState : EnemyState
{
    public override void StartState(EnemyStateMachine owner)
    {
        owner.CrowdControl.interrupt.Interrupt();
        owner.EnemyAnimator.Play("Stagger");
    }

    public override void UpdateState(EnemyStateMachine owner)
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
                owner.enemy.PatienceBar.Patience.Value = 0;
                owner.IsPlayerInRange = false;
                owner.Target = null;
                owner.SetState(EnemyStateMachine.State.Reset);
            }
            else
            {
                owner.SetState(EnemyStateMachine.State.Idle);
            }
        }
    }

    public override void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}