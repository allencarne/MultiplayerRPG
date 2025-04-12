using Unity.Netcode;
using UnityEngine;

public abstract class EnemyAbility : NetworkBehaviour
{
    public abstract void AbilityStart(EnemyStateMachine owner);

    public abstract void AbilityUpdate(EnemyStateMachine owner);

    public abstract void AbilityFixedUpdate(EnemyStateMachine owner);
}
