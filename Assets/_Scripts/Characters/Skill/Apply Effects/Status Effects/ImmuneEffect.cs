using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Immune Effect")]
public class ImmuneEffect : ApplyEffect
{
    public float Duration;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        Buffs buffs = target.GetComponent<Buffs>();
        if (buffs == null) return;

        buffs.immune.StartImmune(Duration);
    }
}
