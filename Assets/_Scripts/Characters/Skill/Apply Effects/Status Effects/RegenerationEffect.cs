using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Regeneration Effect")]
public class RegenerationEffect : ApplyEffect
{
    public int Stacks = 1;
    public float Duration;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        Buffs buffs = target.GetComponent<Buffs>();
        if (buffs == null) return;

        buffs.regeneration.StartRegen(Stacks, Duration);
    }
}
