using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Crowd Control/Immobilize")]
public class ImmobilizeEffect : ApplyEffect
{
    [Header("Duration")]
    public float Duration;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        CrowdControl cc = target.GetComponent<CrowdControl>();
        cc?.immobilize.StartImmobilize(Duration);
    }
}
