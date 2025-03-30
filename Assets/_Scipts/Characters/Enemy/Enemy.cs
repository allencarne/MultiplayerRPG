using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : NetworkBehaviour, IDamageable
{
    [Header("Health")]
    public float Health;
    public float MaxHealth;
    [Header("Speed")]
    public float BaseSpeed;
    public float CurrentSpeed;
    [Header("Damage")]
    public int BaseDamage;
    public int CurrentDamage;
    [Header("AttackSpeed")]
    public float BaseAttackSpeed;
    public float CurrentAttackSpeed;
    [Header("CDR")]
    public float BaseCDR;
    public float CurrentCDR;
    [Header("Armor")]
    public float BaseArmor;
    public float CurrentArmor;

    [Header("Exp")]
    public float expToGive;

    [Header("Idle")]
    protected float idleTime;
    protected Vector2 startingPosition;
    int attemptsCount;

    [Header("Wander")]
    Vector2 newWanderPosition;

    [Header("Chase")]
    protected Transform target;
    public float patience;
    float patienceTime;

    [Header("Bools")]
    bool canSpawn = true;
    bool isPlayerInRange = false;

    [Header("Components")]
    [SerializeField] EnemyHealthBar healthBar;
    protected Rigidbody2D enemyRB;
    protected Animator enemyAnimator;
    [SerializeField] protected Image patienceBar;
    //[SerializeField] protected Image castBar;

    protected enum EnemyState
    {
        Spawn,
        Idle,
        Wander,
        Chase,
        Basic,
        Mobility,
        Special,
        Reset,
        Hurt,
        Death
    }

    protected EnemyState enemyState = EnemyState.Spawn;

    private void Awake()
    {
        enemyAnimator = GetComponentInChildren<Animator>();
        enemyRB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Set Health
        Health = MaxHealth;

        // Set Starting Position
        startingPosition = transform.position;

        // Set Speed
        CurrentSpeed = BaseSpeed;

        // Set Damage
        CurrentDamage = BaseDamage;

        // Set Attack Speed
        CurrentAttackSpeed = BaseAttackSpeed;

        // Set CDR
        CurrentCDR = BaseCDR;

        // Set Armor
        CurrentArmor = BaseArmor;
    }

    private void Update()
    {
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
            case EnemyState.Mobility:
                MobilityState();
                break;
            case EnemyState.Special:
                SpecialState();
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
        if (canSpawn)
        {
            canSpawn = false;

            enemyAnimator.Play("Spawn");
            StartCoroutine(SpawnTimer());
        }
    }

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(.6f);

        enemyState = EnemyState.Idle;
        canSpawn = true;
    }

    protected virtual void IdleState()
    {
        enemyAnimator.Play("Idle");

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
            }

            // Reset the idle time and update the attempts count
            idleTime = 0;
            attemptsCount++;
        }

        if (isPlayerInRange)
        {
            attemptsCount = 0;
            idleTime = 0;
            enemyState = EnemyState.Chase;
        }
    }

    private void WanderState()
    {

    }

    private void ChaseState()
    {

    }

    private void BasicState()
    {

    }

    private void MobilityState()
    {

    }

    private void SpecialState()
    {

    }

    protected virtual void ResetState()
    {

    }

    private void HurtState()
    {

    }

    private void DeathState()
    {

    }

    public void TakeDamage(float damage)
    {
        float damageAfterArmor = Mathf.Max(damage - CurrentArmor, 0);
        Health = Mathf.Max(Health - damageAfterArmor, 0);
        healthBar.UpdateHealth(Health);

        idleTime = 0;

        if (Health <= 0)
        {
            enemyState = EnemyState.Death;
        }
    }

    public void HealEnemy(float heal)
    {
        Health = Mathf.Min(Health + heal, MaxHealth);
        healthBar.UpdateHealth(Health);
    }

    public override void OnNetworkSpawn()
    {

    }
}
