using System.Collections;
using UnityEngine;


public class PlayerSpawnState : PlayerState
{
    public PlayerSpawnState(PlayerStateMachine owner) : base(owner) { }

    public override void EnterState()
    {
        // Spawn Event
        owner.OnSpawn?.Invoke();

        // Play the idle animation in spawn state
        owner.Animator.PlayAnimation("Idle", owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value, owner.customization.WeaponAnimType);

        //owner.Animator.PlayAnimation("Spawn", owner.customization.net_ChestIndex.Value, owner.customization.net_LegsIndex.Value, owner.customization.WeaponAnimType);

        // Play spawn animation
        //owner.PlayerHeadAnimator.Play("Spawn");
        //owner.BodyAnimator.Play("Spawn");
        //owner.ChestAnimator.Play("Spawn");
        //owner.LegsAnimator.Play("Spawn");
        //owner.WeaponAnimator.Play(owner.customization.WeaponAnimType + " Spawn");

        // Face Down
        owner.Animator.SetDirection(new Vector2(0, -1));
        owner.customization.net_FacingDirection.Value = new Vector2(0, -1);

        //owner.PlayerHeadAnimator.SetFloat("Vertical", -1);
        //owner.BodyAnimator.SetFloat("Vertical", -1);
        //owner.ChestAnimator.SetFloat("Vertical", -1);
        //owner.LegsAnimator.SetFloat("Vertical", -1);
        //owner.WeaponAnimator.SetFloat("Vertical", -1);

        // Start the duration coroutine
        owner.StartCoroutine(Duration(owner));
    }

    IEnumerator Duration(PlayerStateMachine owner)
    {
        yield return new WaitForSeconds(.2f);
        owner.IsFullySpawned = true;
        owner.SetState(new PlayerIdleState(owner));
    }
}
