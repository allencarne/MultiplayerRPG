using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/Status Effect/Crowd Control/Incapacitate")]
public class IncapacitateEffect : ApplyEffect
{
    [Header("Duration")]
    public float Duration;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        // Stuns for now - Eventuall add Incap logic
        IStunnable cc = target.GetComponentInChildren<IStunnable>();
        cc?.StartStun(Duration);
    }
}
