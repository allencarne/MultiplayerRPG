using UnityEngine;

public class FlickerShot : PlayerAbility
{
    public override void StartAbility(PlayerStateMachine owner)
    {
        Debug.Log("FlickerShot");
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {

    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

    }
}
