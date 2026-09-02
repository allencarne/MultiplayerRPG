using Unity.Netcode;
using UnityEngine;

public class SkillEffectRelay : NetworkBehaviour
{
    SkillEffect[] onTriggerEffects;
    SkillContext context;
    StateMachine owner;
    bool ignorePlayer, ignoreEnemy, ignoreNPC;
    bool isBreakable;
    bool hasBroken;
    int obstacleLayer;

    [Header("Sparks")]
    [SerializeField] GameObject hitSpark;
    [SerializeField] GameObject hitSpark_Special;

    private void Awake()
    {
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
    }

    public void Initialize(StateMachine _owner, SkillContext _ctx, SkillEffect[] _triggerEffects, bool _ignorePlayer, bool _ignoreEnemy, bool _ignoreNPC, bool _isBreakable)
    {
        owner = _owner;
        context = _ctx;
        onTriggerEffects = _triggerEffects;
        ignorePlayer = _ignorePlayer;
        ignoreEnemy = _ignoreEnemy;
        ignoreNPC = _ignoreNPC;
        isBreakable = _isBreakable;
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
        if (hitObj == attacker) return;

        // Don't take Damage if Immune
        Buffs buffs = collision.GetComponent<Buffs>();
        if (buffs != null && buffs.immune.net_IsImmune.Value) return;

        // create a new context for the trigger effects
        SkillContext triggerCtx = context;
        triggerCtx.Target = hitObj;

        // Execute all effects
        foreach (SkillEffect effect in onTriggerEffects) effect.Execute(owner, triggerCtx);


        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            HitSparkClientRPC(hitPosition, rotation, collision.transform.position);
        }
    }

    [ClientRpc]
    void HitSparkClientRPC(Vector2 hitPosition, Quaternion rotation, Vector2 collisionPosition)
    {
        Instantiate(hitSpark, hitPosition, rotation);

        if (hitSpark_Special) Instantiate(hitSpark_Special, collisionPosition, rotation);
    }
}
