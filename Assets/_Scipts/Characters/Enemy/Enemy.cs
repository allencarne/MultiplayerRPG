using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
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

    protected Vector2 startingPosition;
    bool canSpawn = false;

    [Header("Components")]
    protected Rigidbody2D enemyRB;
    protected Animator enemyAnimator;

    [SerializeField] GameObject spawnEffect;

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

    public void TakeDamage(float damage)
    {

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
            Instantiate(spawnEffect, transform.position, Quaternion.identity);
            StartCoroutine(SpawnTimer());
        }
    }

    IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(.6f);

        enemyState = EnemyState.Idle;
        canSpawn = true;
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

    }

    private void MobilityState()
    {

    }

    private void SpecialState()
    {

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
