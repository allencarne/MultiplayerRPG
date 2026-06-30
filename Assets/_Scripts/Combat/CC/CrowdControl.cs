using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CrowdControl : NetworkBehaviour
{
    public CC_Disarm disarm;
    public CC_Silence silence;
    public CC_Immobilize immobilize;
    public CC_Stun stun;
    public CC_Incapacitate incapacitate;
    public CC_KnockBack knockBack;
    public CC_KnockUp knockUp;
    public CC_Pull pull;
    public CC_Interrupt interrupt;

    public bool IsCrowdControlled =>
    stun.IsStunned ||
    knockBack.IsKnockedBack ||
    knockUp.IsKnockedUp ||
    pull.IsPulled;

    [HideInInspector] public UnityEvent OnStagger;
    [HideInInspector] public UnityEvent OnStaggerEnd;
}
