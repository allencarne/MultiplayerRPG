using UnityEngine;
using System.Collections;

public class EnemyStateMachine : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Enemy enemy;
    [SerializeField] Rigidbody2D enemyRB;
    [SerializeField] Animator enemyAnimator;
    [SerializeField] EnemyAbility enemyBasicAbility;
    [SerializeField] EnemyAbility enemySpecialAbility;
    [SerializeField] EnemyAbility enemyUltimateAbility;

    Transform target;

    [Header("Variables")]
    float idleTime;
    int attemptsCount;

    [SerializeField] float wanderRadius;
    [SerializeField] float basicRadius;
    [SerializeField] float specialRadius;
    [SerializeField] float ultimateRadius;
    [SerializeField] float deAggroRadius;

    bool playerInRange;
    public bool CanBasic = true;
    public bool CanSpecial = true;
    public bool CanUltimate = true;

    Vector2 startingPosition;
    Vector2 wanderPosition;

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

    private void Start()
    {
        Enter_SpawnState();

        startingPosition = transform.position;
    }

    private void Update()
    {
        //Debug.Log(enemyState);

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
                Fixed_WanderState();
                break;
            case EnemyState.Chase:
                Fixed_ChaseState();
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

    #region Spawn

    void Enter_SpawnState()
    {
        Debug.Log("Enter Spawn");

        enemyAnimator.Play("Spawn");
        StartCoroutine(Delay_SpawnState());
    }

    void SpawnState()
    {
        Debug.Log("Spawn Update");
    }

    #endregion

    #region Idle

    void Enter_IdleState()
    {
        Debug.Log("Enter Idle");

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
                Enter_WanderState();
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
            Enter_ChaseState();
        }
    }

    #endregion

    #region Wander

    void Enter_WanderState()
    {
        Debug.Log("Enter Wander");

        enemyAnimator.Play("Wander");

        float minWanderDistance = 1f; // Minimum distance away
        wanderPosition = GetRandomPointInCircle(startingPosition, minWanderDistance, wanderRadius);
    }

    void WanderState()
    {
        Debug.Log("Wander Update");

        if (Vector2.Distance(transform.position, wanderPosition) <= 0.1f)
        {
            Debug.Log("Reached Wander Position -> Transition to Idle");
            enemyRB.linearVelocity = Vector2.zero;
            enemyState = EnemyState.Idle;
            Enter_IdleState();
        }
    }

    void Fixed_WanderState()
    {
        Debug.Log("Wander Fixed");

        // Move to New Wander Position
        Vector2 direction = (wanderPosition - (Vector2)transform.position).normalized;
        enemyRB.linearVelocity = direction * enemy.BaseSpeed;
    }

    Vector2 GetRandomPointInCircle(Vector2 startingPosition, float minDistance, float maxRadius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float randomRadius = Random.Range(minDistance, maxRadius);
        Vector2 randomPoint = startingPosition + new Vector2(Mathf.Cos(angle) * randomRadius, Mathf.Sin(angle) * randomRadius);
        return randomPoint;
    }

    #endregion

    #region Chase

    void Enter_ChaseState()
    {
        Debug.Log("Enter Chase");

        enemyAnimator.Play("Chase");
    }

    void ChaseState()
    {
        enemy.UpdatePatienceBar();

        if (target)
        {
            // Attacks
            float distanceToTarget = Vector2.Distance(transform.position, target.position);
            if (distanceToTarget <= basicRadius)
            {
                if (CanBasic)
                {
                    //enemyState = EnemyState.Basic;
                    //Enter_BasicState();
                }
            }

            // De-Aggro
            float distanceToStartingPosition = Vector2.Distance(startingPosition, target.position);
            if (distanceToStartingPosition >= deAggroRadius)
            {
                enemy.CurrentPatience += Time.deltaTime;

                if (enemy.CurrentPatience >= enemy.TotalPatience)
                {
                    enemy.CurrentPatience = 0f;
                    playerInRange = false;
                    target = null;
                    enemyState = EnemyState.Reset;
                    Enter_ResetState();
                }
            }
            else
            {
                enemy.CurrentPatience = 0;
            }
        }
        else
        {
            // If Target drops during the Chase State
            playerInRange = false;
            enemy.CurrentPatience = 0;
            enemyState = EnemyState.Reset;
            Enter_ResetState();
        }
    }

    void Fixed_ChaseState()
    {
        if (target)
        {
            // Move to Target Position
            Vector2 direction = (target.position - transform.position).normalized;
            enemyRB.linearVelocity = direction * enemy.BaseSpeed;
        }
    }

    #endregion

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

    void Enter_ResetState()
    {

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
        Enter_IdleState();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
            playerInRange = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(startingPosition, wanderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, basicRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, specialRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ultimateRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(startingPosition, deAggroRadius);

    }
}
