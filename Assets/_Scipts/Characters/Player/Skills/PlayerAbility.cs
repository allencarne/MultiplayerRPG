using Unity.Netcode;
using UnityEngine;

public abstract class PlayerAbility : NetworkBehaviour
{
    public Sprite SkillIcon;
    public abstract void StartAbility(PlayerStateMachine owner);

    public abstract void UpdateAbility(PlayerStateMachine owner);

    public abstract void FixedUpdateAbility(PlayerStateMachine owner);

    public virtual void Impact(PlayerStateMachine owner)
    {
        
    }
}
