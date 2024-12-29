using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Start()
    {
        stateMachine.SwordAnimator.Play("Idle");
        stateMachine.BodyAnimator.Play("Idle");
        stateMachine.EyeAnimator.Play("Idle");
        stateMachine.HairAnimator.Play("Idle_" + stateMachine.Player.hairIndex);
    }

    public override void Update()
    {
        // Transitions
        stateMachine.Roll(stateMachine.InputHandler.RollInput);
        stateMachine.BasicAbility(stateMachine.InputHandler.BasicAbilityInput);
        stateMachine.OffensiveAbility(stateMachine.InputHandler.OffensiveAbilityInput);
        stateMachine.MobilityAbility(stateMachine.InputHandler.MobilityAbilityInput);
    }

    public override void FixedUpdate()
    {
        // Transition to Move State
        if (stateMachine.InputHandler.MoveInput != Vector2.zero)
        {
            stateMachine.SetState(new PlayerRunState(stateMachine));
        }
    }
}
