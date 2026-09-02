using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Crowd Control/Disarm")]
public class DisarmEffect : ApplyEffect
{
    [Header("Duration")]
    public float Duration;

    protected override void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx)
    {
        CrowdControl cc = target.GetComponent<CrowdControl>();
        cc?.disarm.StartDisarm(Duration);
    }
}
