using UnityEngine;

public class FrailSlash : PlayerAbility
{
    public override void StartAbility(PlayerStateMachine owner)
    {
        Debug.Log("Start");
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        Debug.Log("Update");
    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {
        Debug.Log("Fixed");
    }
}
