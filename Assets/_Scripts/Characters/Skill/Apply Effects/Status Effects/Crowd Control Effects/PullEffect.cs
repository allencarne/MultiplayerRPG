using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Skill/Skill Effects/Status Effect/Crowd Control/Pull")]
public class PullEffect : ApplyEffect
{
    [Header("Pull Force")]
    public float Force;

    [Header("Pull Duration")]
    public float Duration;

    protected override void ApplyTo(NetworkObject target, StateMachine owner, SkillContext ctx)
    {
        IPullable cc = target.GetComponent<IPullable>();
        if (cc == null) return;

        Vector2 direction = ((Vector2)target.transform.position - (Vector2)owner.transform.position).normalized;

        cc.Pull(-direction, Force, Duration);
    }
}
