using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Immovable Effect")]
public class ImmovableEffect : ApplyEffect
{
    public float Duration;

    protected override void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx)
    {
        Buffs buffs = target.GetComponent<Buffs>();
        if (buffs == null) return;

        buffs.immoveable.StartImmovable(Duration);
    }
}
