using UnityEngine;

public abstract class NPCState : MonoBehaviour
{
    protected NPCStateMachine owner;

    public virtual void StartState(NPCStateMachine owner)
    {

    }

    public virtual void UpdateState(NPCStateMachine owner)
    {

    }

    public virtual void FixedUpdateState(NPCStateMachine owner)
    {

    }
}
