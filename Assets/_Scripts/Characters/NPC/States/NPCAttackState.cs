
public class NPCAttackState : NPCState
{
    private readonly ActiveSkill skill;

    public NPCAttackState(NPCStateMachine owner, ActiveSkill skill) : base(owner)
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
