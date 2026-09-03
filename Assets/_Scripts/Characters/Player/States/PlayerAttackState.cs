using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private readonly ActiveSkill skill;

    public PlayerAttackState(PlayerStateMachine owner, ActiveSkill skill) : base(owner)
    {
        this.skill = skill;
    }

    public override void EnterState()
    {
        owner.CurrentSkill = skill;
        skill.StartSkill(owner);
    }

    public override void UpdateState()
    {
        owner.CurrentSkill?.UpdateSkill(owner);
    }

    public override void FixedUpdateState()
    {
        owner.CurrentSkill?.FixedUpdateSkill(owner);
    }
}
