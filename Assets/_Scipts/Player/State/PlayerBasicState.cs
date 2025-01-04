using UnityEngine;

public class PlayerBasicState : PlayerState
{
    public PlayerBasicState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Start()
    {
        //stateMachine.SwordAnimator.Play("Idle");
        stateMachine.BodyAnimator.Play("Sword_Attack_C");
        //stateMachine.EyeAnimator.Play("Idle");
        //stateMachine.HairAnimator.Play("Idle_" + stateMachine.Player.hairIndex);
    }

    public override void Update()
    {
        
    }
}
