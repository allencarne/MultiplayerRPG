using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private readonly PlayerSkill skill;

    public PlayerAttackState(PlayerStateMachine owner, PlayerSkill skill) : base(owner)
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
