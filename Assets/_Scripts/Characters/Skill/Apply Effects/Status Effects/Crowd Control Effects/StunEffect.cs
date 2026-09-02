using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Crowd Control/Stun")]
public class StunEffect : ApplyEffect
{
    [Header("Duration")]
    public float Duration;

    protected override void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx)
    {
        IStunnable cc = target.GetComponentInChildren<IStunnable>();
        cc?.StartStun(Duration);
    }
}
