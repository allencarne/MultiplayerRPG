
public abstract class PlayerState
{
    protected PlayerStateMachine stateMachine;

    protected PlayerState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void Update();

    public virtual void FixedUpdate()
    {

    }
}
