using Unity.Netcode;
using UnityEngine;

public abstract class StateMachine : NetworkBehaviour
{
    public CharacterAnimator Animator;
    public Collider2D Collider2D;
    public Rigidbody2D RigidBody2D;

    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;
    public Mobility Mobility;

    private void Awake()
    {
        Animator = GetComponentInChildren<CharacterAnimator>();
        Collider2D = GetComponent<Collider2D>();
        RigidBody2D = GetComponent<Rigidbody2D>();

        CrowdControl = GetComponent<CrowdControl>();
        Buffs = GetComponent<Buffs>();
        DeBuffs = GetComponent<DeBuffs>();
    }
}
