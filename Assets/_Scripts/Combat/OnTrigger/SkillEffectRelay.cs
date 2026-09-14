using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillEffectRelay : NetworkBehaviour
{
    SkillEffect[] onTriggerEffects;
    SkillContext context;
    StateMachine owner;
    bool ignorePlayer, ignoreEnemy, ignoreNPC;
    bool ignoreAttacker = true;
    bool singleUsePerTarget = true;
    bool isBreakable;
    bool hasBroken;
    int obstacleLayer;

    // track targets we've already applied effects to (prevents re-triggering)
    HashSet<ulong> triggeredTargets = new HashSet<ulong>();

    [Header("Sparks")]
    GameObject Spark;
    GameObject SpecialSpark;

    private void Awake()
    {
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    public void Initialize(StateMachine _owner, SkillContext _ctx, SkillEffect[] _triggerEffects, bool _ignorePlayer, bool _ignoreEnemy, bool _ignoreNPC, bool _ignoreAttacker, bool _singleUsePerTarget, bool _isBreakable, GameObject spark = null, GameObject specialSpark = null)
    {
        owner = _owner;
        context = _ctx;
        onTriggerEffects = _triggerEffects;
        ignorePlayer = _ignorePlayer;
        ignoreEnemy = _ignoreEnemy;
        ignoreNPC = _ignoreNPC;
        ignoreAttacker = _ignoreAttacker;
        singleUsePerTarget = _singleUsePerTarget;
        isBreakable = _isBreakable;

        if (spark != null) Spark = spark;
        if (specialSpark != null) SpecialSpark = specialSpark;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Only the server can apply effects
        if (!IsServer) return;
        if (hasBroken) return;

        // Return if there are no effects
        if (onTriggerEffects == null || onTriggerEffects.Length == 0) return;

        // Return if invalid target
        if (collision.CompareTag("Player") && ignorePlayer) return;
        if (collision.CompareTag("Enemy") && ignoreEnemy) return;
        if (collision.CompareTag("NPC") && ignoreNPC) return;

        // Get Hit Object and Attacker
        NetworkObject hitObj = collision.GetComponent<NetworkObject>();
        NetworkObject attacker = owner.GetComponent<NetworkObject>();

        // Calculate
        Vector2 hitPosition = collision.ClosestPoint(transform.position);
        Vector2 attackerPosition = attacker.transform.position;
        Vector2 direction = (hitPosition - attackerPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Break and Spark
        if (collision.gameObject.layer == obstacleLayer)
        {
            if (isBreakable)
            {
                hasBroken = true;
                HitSparkClientRPC(hitPosition, rotation, collision.transform.position);
                NetworkObject.Despawn(true);
            }
        }

        // Prevents self-hit
        if (hitObj == null || attacker == null) return;
        if (ignoreAttacker && hitObj == attacker) return;

        // If configured to only trigger once per target, skip if already triggered
        if (singleUsePerTarget && hitObj != null && triggeredTargets.Contains(hitObj.NetworkObjectId)) return;

        // create a new context for the trigger effects
        SkillContext triggerCtx = context;
        triggerCtx.Target = hitObj;

        // Execute all effects
        foreach (SkillEffect effect in onTriggerEffects) effect.Execute(owner, triggerCtx);

        // mark target as triggered
        if (singleUsePerTarget && hitObj != null) triggeredTargets.Add(hitObj.NetworkObjectId);

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            HitSparkClientRPC(hitPosition, rotation, collision.transform.position);
        }
    }

    [ClientRpc]
    void HitSparkClientRPC(Vector2 hitPosition, Quaternion rotation, Vector2 collisionPosition)
    {
        if (Spark) Instantiate(Spark, hitPosition, rotation);
        if (SpecialSpark) Instantiate(SpecialSpark, collisionPosition, rotation);
    }
}
