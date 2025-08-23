using UnityEngine;

public abstract class PlayerState: MonoBehaviour
{
    public abstract void StartState(PlayerStateMachine owner);

    public abstract void UpdateState(PlayerStateMachine owner);

    public abstract void FixedUpdateState(PlayerStateMachine owner);
}
