using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/Debuff Effect")]
public class DebuffEffect : ApplyEffect
{
    public enum DebuffType { Slow, Weakness, Impede, Vulnerability, Exhaust }
    public DebuffType Debuff;
    public int Stacks = 1;
    public float Duration;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        DeBuffs debuffs = target.GetComponent<DeBuffs>();
        if (debuffs == null) return;

        switch (Debuff)
        {
            case DebuffType.Slow: debuffs.slow.StartSlow(Stacks, Duration); break;
            case DebuffType.Weakness: debuffs.weakness.StartWeakness(Stacks, Duration); break;
            case DebuffType.Impede: debuffs.impede.StartImpede(Stacks, Duration); break;
            case DebuffType.Vulnerability: debuffs.vulnerability.StartVulnerability(Stacks, Duration); break;
            case DebuffType.Exhaust: debuffs.exhaust.StartExhaust(Stacks, Duration); break;
        }
    }
}
