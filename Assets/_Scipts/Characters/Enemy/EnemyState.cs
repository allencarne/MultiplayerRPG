using UnityEngine;

public abstract class EnemyState : MonoBehaviour
{
    protected EnemyStateMachine owner;

    public virtual void StartState(EnemyStateMachine owner)
    {

    }

    public virtual void UpdateState(EnemyStateMachine owner)
    {

    }

    public virtual void FixedUpdateState(EnemyStateMachine owner)
    {

    }
}
