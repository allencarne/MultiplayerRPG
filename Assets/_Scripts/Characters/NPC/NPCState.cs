using UnityEngine;

public abstract class NPCState
{
    protected NPCStateMachine owner;

    protected NPCState(NPCStateMachine owner)
    {
        this.owner = owner;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
