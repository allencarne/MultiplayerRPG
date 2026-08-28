using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Heal Effect")]
public class HealEffect : ApplyEffect
{
    public int FlatAmount;
    public bool PercentOfDamageDealt;
    [Range(0, 100)] public float LifestealPercent;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        // Get Healable
        IHealable healable = target.GetComponent<IHealable>();
        if (healable == null) return;

        // Calculate Heal Amount
        float amount = PercentOfDamageDealt ? ctx.LastDamageDealt * (LifestealPercent / 100f) : FlatAmount;

        // Give Heal
        healable.GiveHeal(amount, HealType.Flat);
    }
}
