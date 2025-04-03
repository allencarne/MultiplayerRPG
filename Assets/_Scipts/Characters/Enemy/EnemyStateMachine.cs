using UnityEngine;
using System.Collections;
using UnityEngine.Events;

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

    bool playerInRange;
    public bool CanBasic = true;
    public bool CanSpecial = true;
    public bool CanUltimate = true;

    public enum EnemyState
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

    public EnemyState enemyState = EnemyState.Spawn;
    private UnityEvent<EnemyState> OnStateChanged;

    private void Awake()
    {
        // Initialize the UnityEvent
        OnStateChanged = new UnityEvent<EnemyState>();
    }

    private void OnEnable()
    {
        // Subscribe to the event with the appropriate method
        OnStateChanged.AddListener(OnStateEnter);
    }

    private void OnDisable()
    {
        // Unsubscribe from the event
        OnStateChanged.RemoveListener(OnStateEnter);
    }

    private void Start()
    {
        OnStateChanged?.Invoke(enemyState);
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

    private void FixedUpdate()
    {
        switch (enemyState)
        {
            case EnemyState.Spawn:

                break;
            case EnemyState.Idle:

                break;
            case EnemyState.Wander:

                break;
            case EnemyState.Chase:

                break;
            case EnemyState.Basic:

                break;
            case EnemyState.Special:

                break;
            case EnemyState.Ultimate:

                break;
            case EnemyState.Reset:

                break;
            case EnemyState.Hurt:

                break;
            case EnemyState.Death:

                break;
        }
    }

    void OnStateEnter(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Spawn:
                Enter_SpawnState();
                break;
            case EnemyState.Idle:
                Enter_IdleState();
                break;
            case EnemyState.Wander:
                Enter_WanderState();
                break;
        }
    }

    // @@@@@ Spawn @@@@@

    void Enter_SpawnState()
    {
        Debug.Log("Spawn Enter");

        enemyAnimator.Play("Spawn");
        StartCoroutine(Delay_SpawnState());
    }

    void SpawnState()
    {
        Debug.Log("Spawn Update");
    }

    // @@@@@ Idle @@@@@

    float idleTime;
    int attemptsCount;

    void Enter_IdleState()
    {
        Debug.Log("Idle Enter");

        enemyAnimator.Play("Idle");
    }

    void IdleState()
    {
        idleTime += Time.deltaTime;

        if (idleTime >= 5)
        {
            int maxAttempts = 3; // Maximum number of consecutive failed attempts
            int consecutiveFailures = Mathf.Min(attemptsCount, maxAttempts);

            // Calculate the probability of transitioning to the wander state based on the number of consecutive failures
            float wanderProbability = Mathf.Min(0.5f + 0.25f * consecutiveFailures, 1.0f);

            // Check if the enemy will transition to the wander state based on the calculated probability
            if (Random.value < wanderProbability)
            {
                idleTime = 0;

                enemyState = EnemyState.Wander;
                OnStateChanged?.Invoke(enemyState);
            }

            // Reset the idle time and update the attempts count
            idleTime = 0;
            attemptsCount++;
        }

        if (playerInRange)
        {
            attemptsCount = 0;
            idleTime = 0;
            enemyState = EnemyState.Chase;
            OnStateChanged?.Invoke(enemyState);
        }
    }

    // @@@@@ Wander @@@@@

    void Enter_WanderState()
    {

    }

    void WanderState()
    {

    }

    // @@@@@ Chase @@@@@

    void ChaseState()
    {

    }

    void BasicState()
    {
        if (!CanBasic) return;
        if (enemyBasicAbility == null) return;

        Debug.Log("CanBasic");
        enemyBasicAbility.Activate(this);
    }

    void SpecialState()
    {
        if (!CanSpecial) return;
        if (enemySpecialAbility == null) return;

        Debug.Log("CanSpecial");
        enemySpecialAbility.Activate(this);
    }

    void UltimateState()
    {
        if (!CanUltimate) return;
        if (enemyUltimateAbility == null) return;

        Debug.Log("CanUltimate");
        enemyUltimateAbility.Activate(this);
    }

    void ResetState()
    {

    }

    void HurtState()
    {

    }

    void DeathState()
    {

    }

    IEnumerator Delay_SpawnState()
    {
        yield return new WaitForSeconds(0.6f);
        enemyState = EnemyState.Idle;
        OnStateChanged?.Invoke(enemyState);
    }
}
