using UnityEngine;

public class NPCStateMachine : MonoBehaviour
{
    [Header("Private Components")]
    [SerializeField] NPC npc;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Collider2D Collider;
    [SerializeField] Animator bodyAnimator;
    [SerializeField] Animator eyesAnimator;
    [SerializeField] Animator hairAnimator;

    [Header("Public Components")]
    [HideInInspector] public Transform Target;
    public CrowdControl CrowdControl;
    public Buffs Buffs;
    public DeBuffs DeBuffs;

    public enum State
    {
        Spawn,
        Idle,
        Wander,
        Chase,
        Reset,
        Hurt,
        Death,
        Basic,
        Special,
        Ultimate,
    }

    public State state = State.Spawn;
}
