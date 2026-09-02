using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Bleed Effect")]
public class BleedEffect : ApplyEffect
{
    public int Stacks = 1;
    public float Duration;

    protected override void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx)
    {
        DeBuffs debuffs = target.GetComponent<DeBuffs>();
        if (debuffs == null) return;

        debuffs.bleed.StartBleed(Stacks, Duration);
    }
}
