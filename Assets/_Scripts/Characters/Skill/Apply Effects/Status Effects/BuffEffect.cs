using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Buff Effect")]
public class BuffEffect : ApplyEffect
{
    public enum BuffType { Haste, Might, Alacrity, Protection, Swiftness }
    public BuffType Buff;
    public int Stacks = 1;
    public float Duration;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        Buffs buffs = target.GetComponent<Buffs>();
        if (buffs == null) return;

        switch (Buff)
        {
            case BuffType.Haste: buffs.haste.StartHaste(Stacks, Duration); break;
            case BuffType.Might: buffs.might.StartMight(Stacks, Duration); break;
            case BuffType.Alacrity: buffs.alacrity.StartAlacrity(Stacks, Duration); break;
            case BuffType.Protection: buffs.protection.StartProtection(Stacks, Duration); break;
            case BuffType.Swiftness: buffs.swiftness.StartSwiftness(Stacks, Duration); break;
        }
    }
}
