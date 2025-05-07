using Unity.Netcode;
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

    void GainFury()
    {
        if (_owner.IsServer)
        {
            _owner.player.Fury.Value = Mathf.Min(_owner.player.Fury.Value + 5, _owner.player.MaxFury.Value);
        }
        else
        {
            GainFuryServerRPC(_owner.player.NetworkObject);
        }
    }

    [ServerRpc]
    void GainFuryServerRPC(NetworkObjectReference playerRef)
    {
        if (playerRef.TryGet(out NetworkObject networkObject))
        {
            Player player = networkObject.GetComponent<Player>();
            if (player != null)
            {
                player.Fury.Value = Mathf.Min(player.Fury.Value + 5, player.MaxFury.Value);
            }
        }
    }

    public override void OnDestroy()
    {
        DamageOnTrigger.OnDamageDealt.RemoveListener(GainFury);
    }
}
