using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill Effects/Status Effect/Crowd Control/Knock Back")]
public class KnockBackEffect : ApplyEffect
{
    [Header("KnockBack Force")]
    public float Force;

    [Header("KnockBack Duration")]
    public float Duration;

    protected override void ApplyTo(NetworkObject target, PlayerStateMachine owner, SkillContext ctx)
    {
        IKnockbackable cc = target.GetComponent<IKnockbackable>();
        if (cc == null) return;

        Vector2 direction = ((Vector2)target.transform.position - (Vector2)owner.transform.position).normalized;
        cc.KnockBack(direction, Force, Duration);
    }
}
