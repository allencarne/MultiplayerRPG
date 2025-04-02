using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class EnemyStateMachine : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Enemy enemy;
    [SerializeField] Rigidbody2D enemyRB;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] EnemyAbility enemyBasicAbility;
    [SerializeField] EnemyAbility enemySpecialAbility;
    [SerializeField] EnemyAbility enemyUltimateAbility;

    [Header("Variables")]
    [SerializeField] float patience;
    float idleTime;
    int attemptsCount;
    float patienceTime;

    Vector2 startingPosition;
    Vector2 newWanderPosition;
    Transform target;

    bool canSpawn = true;
    bool isPlayerInRange = false;
    public bool CanBasic = true;
    public bool CanSpecial = true;
    public bool CanUltimate = true;

    protected enum EnemyState
    {
        Spawn,
        Idle,
        Wander,
        Chase,
        Basic,
        Special,
        Ultimate,
        Reset,
        Hurt,
        Death
    }

    protected EnemyState enemyState = EnemyState.Spawn;

    private void Start()
    {
        // Set Starting Position
        startingPosition = transform.position;
    }

    private void Update()
    {
        Debug.Log(enemyState);

        switch (enemyState)
        {
            case EnemyState.Spawn:
                SpawnState();
                break;
            case EnemyState.Idle:
                IdleState();
                break;
            case EnemyState.Wander:
                WanderState();
                break;
            case EnemyState.Chase:
                ChaseState();
                break;
            case EnemyState.Basic:
                BasicState();
                break;
            case EnemyState.Special:
                SpecialState();
                break;
            case EnemyState.Ultimate:
                UltimateState();
                break;
            case EnemyState.Reset:
                ResetState();
                break;
            case EnemyState.Hurt:
                HurtState();
                break;
            case EnemyState.Death:
                DeathState();
                break;
        }
    }

    private void SpawnState()
    {

    }

    private void IdleState()
    {

    }

    private void WanderState()
    {

    }

    private void ChaseState()
    {

    }

    private void BasicState()
    {
        if (!CanBasic) return;
        if (enemyBasicAbility == null) return;

        Debug.Log("CanBasic");
        enemyBasicAbility.Activate(this);
    }

    private void SpecialState()
    {
        if (!CanSpecial) return;
        if (enemySpecialAbility == null) return;

        Debug.Log("CanSpecial");
        enemySpecialAbility.Activate(this);
    }

    private void UltimateState()
    {
        if (!CanUltimate) return;
        if (enemyUltimateAbility == null) return;

        Debug.Log("CanUltimate");
        enemyUltimateAbility.Activate(this);
    }

    private void ResetState()
    {

    }

    private void HurtState()
    {

    }

    private void DeathState()
    {

    }
}
