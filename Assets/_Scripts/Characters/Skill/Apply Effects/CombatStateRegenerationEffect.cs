using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Combat State Regeneration Effect")]
public class CombatStateRegenerationEffect : ApplyEffect
{
    [Header("Out of Combat (when not in combat)")]
    [Tooltip("Stacks to add when leaving combat. Use negative values to remove stacks.")]
    public int OutOfCombatStacks = 1;
    [Tooltip("Duration for out-of-combat stacks. -1 = fixed (no duration).")]
    public float OutOfCombatDuration = -1f;

    [Header("In Combat (when entering combat)")]
    [Tooltip("Stacks to add when entering combat. Use negative values to remove stacks.")]
    public int InCombatStacks = -1;
    [Tooltip("Duration for in-combat stacks. -1 = fixed (no duration).")]
    public float InCombatDuration = -1f;

    protected override void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx)
    {
        Buffs buffs = target.GetComponent<Buffs>();
        if (buffs == null || owner == null || owner.Combat == null) return;

        bool inCombat = owner.Combat.InCombat.Value;

        if (inCombat)
        {
            // Entering combat (or currently in combat) -> apply in-combat stacks (can be negative to remove)
            if (InCombatStacks != 0)
            {
                buffs.regeneration.StartRegen(InCombatStacks, InCombatDuration);
            }
        }
        else
        {
            // Out of combat -> apply out-of-combat stacks (can be negative to remove)
            if (OutOfCombatStacks != 0)
            {
                float currentHp = owner.Stats.net_CurrentHP.Value;
                float maxHp = owner.Stats.net_TotalHP.Value;
                if (currentHp < maxHp)
                {
                    buffs.regeneration.StartRegen(OutOfCombatStacks, OutOfCombatDuration);
                }
            }
        }
    }
}
