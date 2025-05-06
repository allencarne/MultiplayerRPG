using UnityEngine;

public class Fury : PlayerAbility
{
    private PlayerStateMachine _owner;

    public override void StartAbility(PlayerStateMachine owner)
    {
        _owner = owner;
        DamageOnTrigger.OnDamageDealt.AddListener(GainFury);
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {

    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

    }

    public void GainFury()
    {
        _owner.player.Fury.Value = Mathf.Min(_owner.player.Fury.Value + 5, _owner.player.MaxFury.Value);
    }

    public override void OnDestroy()
    {
        DamageOnTrigger.OnDamageDealt.RemoveListener(GainFury);
    }
}
