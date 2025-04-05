using Unity.Netcode;
using UnityEngine;

public abstract class EnemyAbility : NetworkBehaviour
{
    protected EnemyStateMachine owner;

    public virtual void AbilityStart(EnemyStateMachine owner)
    {

    }

    public virtual void AbilityUpdate(EnemyStateMachine owner)
    {

    }

    public virtual void AbilityFixedUpdate(EnemyStateMachine owner)
    {

    }
}
