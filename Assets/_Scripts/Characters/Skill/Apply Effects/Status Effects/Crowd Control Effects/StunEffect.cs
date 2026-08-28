using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/Status Effect/Crowd Control/Stun")]
public class StunEffect : ApplyEffect
{
    [Header("Duration")]
    public float Duration;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        IStunnable cc = target.GetComponentInChildren<IStunnable>();
        cc?.StartStun(Duration);
    }
}
