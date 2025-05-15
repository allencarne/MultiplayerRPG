using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Fury : PlayerAbility
{
    float idleTime;
    Coroutine timer;

    public override void StartAbility(PlayerStateMachine owner)
    {
        DamageOnTrigger.OnDamageDealt.AddListener(GainFury);
    }

    public override void UpdateAbility(PlayerStateMachine owner)
    {
        idleTime += Time.deltaTime;

        if (idleTime >= 8 && owner.player.Fury.Value > 0)
        {
            if (timer == null)
            {
                timer = StartCoroutine(DecreaseFury(owner));
            }
        }
        else if (timer != null)
        {
            StopCoroutine(DecreaseFury(owner));
            timer = null;
        }
    }

    IEnumerator DecreaseFury(PlayerStateMachine owner)
    {
        while (owner.player.Fury.Value > 0 && idleTime >= 8)
        {
            owner.player.Fury.Value--;
            yield return new WaitForSeconds(1f);
        }
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
            idleTime = 0;

            if (timer != null)
            {
                StopCoroutine(timer);
                timer = null;
            }
        }
    }

    public override void OnDestroy()
    {
        DamageOnTrigger.OnDamageDealt.RemoveListener(GainFury);
    }
}
