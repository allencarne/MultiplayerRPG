using Unity.Netcode;
using UnityEngine;

public abstract class EnemyAbility : NetworkBehaviour
{
    public abstract void Activate(EnemyStateMachine owner);
}
