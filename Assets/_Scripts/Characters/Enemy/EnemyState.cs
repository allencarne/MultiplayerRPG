using UnityEngine;

public abstract class EnemyState
{
    protected EnemyStateMachine owner;

    protected EnemyState(EnemyStateMachine owner)
    {
        this.owner = owner;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
