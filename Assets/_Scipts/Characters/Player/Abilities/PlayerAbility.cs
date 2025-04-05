using Unity.Netcode;

public abstract class PlayerAbility : NetworkBehaviour
{
    public abstract void StartAbility(PlayerStateMachine owner);

    public abstract void UpdateAbility(PlayerStateMachine owner);

    public abstract void FixedUpdateAbility(PlayerStateMachine owner);
}
