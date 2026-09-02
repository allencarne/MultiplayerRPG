using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Crowd Control/Interrupt")]
public class InterruptEffect : ApplyEffect
{
    protected override void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx)
    {
        CrowdControl cc = target.GetComponent<CrowdControl>();
        cc?.interrupt.Interrupt();
    }
}
