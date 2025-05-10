using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Fury : PlayerAbility
{
    private PlayerStateMachine _owner;

    public override void StartAbility(PlayerStateMachine owner)
    {
        DamageOnTrigger.OnDamageDealt.AddListener(GainFury);
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {

    }

    public override void FixedUpdateAbility(PlayerStateMachine owner)
    {

    }

    void GainFury(NetworkObject attacker)
    {
        PlayerStateMachine owner = attacker.GetComponent<PlayerStateMachine>();
        if (owner != null)
        {
            owner.player.Fury.Value = Mathf.Min(owner.player.Fury.Value + 5, owner.player.MaxFury.Value);
        }
    }

    public override void OnDestroy()
    {
        DamageOnTrigger.OnDamageDealt.RemoveListener(GainFury);
    }
}
