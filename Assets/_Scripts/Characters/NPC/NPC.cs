using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class NPC : NetworkBehaviour, IInteractable
{
    [Header("Data")]
    public NPCData Data;
    public CharacterCustomizationData Custom;

    [Header("Components")]
    public NPCHead npcHead;
    [SerializeField] NPCStateMachine stateMachine;
    public CharacterStats stats;
    [SerializeField] NPCQuest npcQuest;
    [SerializeField] NPCDialogue npcDialogue;

    [Header("UI")]
    public PatienceBar PatienceBar;
    public CastBar CastBar;

    [Header("Variables")]
    public string DisplayName => Data.NPCName;
    public bool IsDead;
    Vector2 facingDirection = new Vector2(0, -1);

    [Header("Combat")]
    public bool InCombat = false;
    public bool IsRegen;
    float CombatTime = 0;
    Coroutine combatTimerCoroutine;

    [Header("Sprites")]
    public SpriteRenderer NPCHeadSprite;
    public SpriteRenderer BodySprite;

    public SpriteRenderer HairSprite;
    public SpriteRenderer EyeSprite;
    public SpriteRenderer HelmSprite;

    public SpriteRenderer SwordSprite;
    public SpriteRenderer ShadowSprite;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            stats.net_TotalHP.Value = Data.MaxHealth;
            stats.net_BaseHP.Value = Data.MaxHealth;
            stats.net_CurrentHP.Value = Data.MaxHealth;
        }

        stats.OnEnemyDamaged.AddListener(TargetAttacker);
        stats.OnDeath.AddListener(DeathState);

        stats.OnDamaged.AddListener(TakeDamage);
        stats.OnDamageDealt.AddListener(DealDamage);

        stats.net_CurrentHP.OnValueChanged += OnHPChanged;
        stats.net_TotalHP.OnValueChanged += OnMaxHPChanged;
    }

    public override void OnNetworkDespawn()
    {
        stats.OnEnemyDamaged.RemoveListener(TargetAttacker);
        stats.OnDeath.RemoveListener(DeathState);

        stats.OnDamaged.RemoveListener(TakeDamage);
        stats.OnDamageDealt.RemoveListener(DealDamage);

        stats.net_CurrentHP.OnValueChanged -= OnHPChanged;
        stats.net_TotalHP.OnValueChanged -= OnMaxHPChanged;

        if (combatTimerCoroutine != null)
        {
            StopCoroutine(combatTimerCoroutine);
        }
    }
    private void Start()
    {
        NPCHeadSprite.color = Custom.skinColors[Data.skinColorIndex];
        BodySprite.color = Custom.skinColors[Data.skinColorIndex];

        HairSprite.color = Custom.hairColors[Data.hairColorIndex];

        Material eyemat = EyeSprite.material;
        eyemat.SetColor("_NewColor", Custom.eyeColors[Data.eyeColorIndex]);

        npcHead.SetEyes(facingDirection);
        npcHead.SetHair(facingDirection);
        npcHead.SetHelm(facingDirection);

        SwordSprite.sprite = Data.Weapon;

        stats.BaseSpeed = Data.Speed;
        stats.BaseDamage = Data.Damage;
        stats.BaseAS = Data.AttackSpeed;
        stats.BaseCDR = Data.CoolDownRecution;
        stats.BaseArmor = Data.Armor;
    }

    void OnHPChanged(float previousValue, float newValue)
    {
        UpdateRegeneration();
    }

    void OnMaxHPChanged(float previousValue, float newValue)
    {
        UpdateRegeneration();
    }

    void UpdateRegeneration()
    {
        if (IsRegen && (stats.net_CurrentHP.Value >= stats.net_TotalHP.Value || InCombat))
        {
            IsRegen = false;
            stateMachine.Buffs.regeneration.StartRegen(-1, -1);
        }
    }

    IEnumerator CombatTimer()
    {
        CombatTime = 0f;

        while (CombatTime < 10f)
        {
            CombatTime += Time.deltaTime;
            yield return null;
        }

        // Combat timer expired
        InCombat = false;
        CombatTime = 0;
        combatTimerCoroutine = null;

        // Start regen if health is not full
        if (stats.net_CurrentHP.Value < stats.net_TotalHP.Value)
        {
            IsRegen = true;
            stateMachine.Buffs.regeneration.StartRegen(1, -1);
        }
    }

    public void Interact(PlayerInteract player)
    {
        PlayerQuest playerQuest = player.GetComponentInParent<PlayerQuest>();
        Quest quest = npcQuest?.GetAvailableQuest(playerQuest);

        if (quest != null)
        {
            // Quest
            player.OpenQuestUI(quest, this);
            return;
        }

        if (Data.Items != null && Data.Items.Length != 0)
        {
            player.OpenShopUI(Data);
            return;
        }

        // Dialogue
        player.OpenDialogueUI(Data.NPCName, npcDialogue);
    }

    void DeathState()
    {
        stateMachine.SetState(NPCStateMachine.State.Death);
    }

    void TargetAttacker(NetworkObject attackerID)
    {
        if (stateMachine.state == NPCStateMachine.State.Reset) return;
        if (!attackerID.gameObject.CompareTag("Enemy")) return;

        if (stateMachine.Target == null)
        {
            stateMachine.Target = attackerID.transform;
            stateMachine.IsEnemyInRange = true;
        }

        if (stateMachine.Target != null && stateMachine.SecondTarget == null)
        {
            stateMachine.SecondTarget = attackerID.transform;
            stateMachine.IsEnemyInRange = true;
        }
    }

    void TakeDamage(float damage)
    {
        if (Data.npcClass == NPCClass.Patrol)
        {
            EnterCombat();
        }

        if (!IsRegen) return;
        IsRegen = false;
        stateMachine.Buffs.regeneration.StartRegen(-1, -1);
    }

    void DealDamage()
    {
        if (Data.npcClass == NPCClass.Patrol)
        {
            EnterCombat();
        }

        if (!IsRegen) return;
        IsRegen = false;
        stateMachine.Buffs.regeneration.StartRegen(-1, -1);
    }

    void EnterCombat()
    {
        InCombat = true;
        CombatTime = 0;

        // Restart combat timer
        if (combatTimerCoroutine != null)
        {
            StopCoroutine(combatTimerCoroutine);
        }
        combatTimerCoroutine = StartCoroutine(CombatTimer());
    }
}
