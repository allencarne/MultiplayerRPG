using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/Stun Effect")]
public class StunEffect : ApplyEffect
{
    public float Duration;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        IStunnable stunnable = target.GetComponentInChildren<IStunnable>();
        stunnable?.StartStun(Duration);
    }
}
