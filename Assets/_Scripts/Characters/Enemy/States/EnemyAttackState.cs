using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private readonly EnemySkill skill;

    public EnemyAttackState(EnemyStateMachine owner, EnemySkill skill) : base(owner)
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
