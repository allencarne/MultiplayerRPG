public abstract class PlayerState
{
    protected PlayerStateMachine owner;

    protected PlayerState(PlayerStateMachine owner)
    {
        this.owner = owner;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
