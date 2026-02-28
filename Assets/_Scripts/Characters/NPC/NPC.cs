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
    public NetworkVariable<Vector2> net_FacingDirection = new(new Vector2(0, -1),NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

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

            stats.net_BaseSpeed.Value = Data.Speed;
            stats.net_BaseDamage.Value = Data.Damage;
            stats.net_BaseAS.Value = Data.AttackSpeed;
            stats.net_BaseCDR.Value = Data.CoolDownRecution;
            stats.net_BaseArmor.Value = Data.Armor;

            stateMachine.Initialize();
        }

        ApplyFacingDirection(net_FacingDirection.Value);
        net_FacingDirection.OnValueChanged += OnFacingDirectionChanged;

        stats.OnEnemyDamaged.AddListener(TargetAttacker);
        stats.OnDeath.AddListener(Death);

        stats.OnDamaged.AddListener(TakeDamage);
        stats.OnDamageDealt.AddListener(DealDamage);

        stats.net_CurrentHP.OnValueChanged += OnHPChanged;
        stats.net_TotalHP.OnValueChanged += OnMaxHPChanged;
    }

    public override void OnNetworkDespawn()
    {
        net_FacingDirection.OnValueChanged -= OnFacingDirectionChanged;

        stats.OnEnemyDamaged.RemoveListener(TargetAttacker);
        stats.OnDeath.RemoveListener(Death);

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

        npcHead.SetEyes(net_FacingDirection.Value);
        npcHead.SetHair(net_FacingDirection.Value);
        npcHead.SetHelm(net_FacingDirection.Value);

        SwordSprite.sprite = Data.Weapon;
    }

    private void Update()
    {
        Debug.Log(stats.net_BaseSpeed.Value);
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

        if (playerQuest != null)
        {
            playerQuest.UpdateObjective(ObjectiveType.Complete, Data.NPC_ID, 1);
        }

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

    void Death()
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

    void OnFacingDirectionChanged(Vector2 previous, Vector2 newDir)
    {
        ApplyFacingDirection(newDir);
    }

    void ApplyFacingDirection(Vector2 dir)
    {
        stateMachine.HeadAnimator.SetFloat("Horizontal", dir.x);
        stateMachine.HeadAnimator.SetFloat("Vertical", dir.y);
        stateMachine.BodyAnimator.SetFloat("Horizontal", dir.x);
        stateMachine.BodyAnimator.SetFloat("Vertical", dir.y);
        stateMachine.ChestAnimator.SetFloat("Horizontal", dir.x);
        stateMachine.ChestAnimator.SetFloat("Vertical", dir.y);
        stateMachine.LegsAnimator.SetFloat("Horizontal", dir.x);
        stateMachine.LegsAnimator.SetFloat("Vertical", dir.y);
        stateMachine.SwordAnimator.SetFloat("Horizontal", dir.x);
        stateMachine.SwordAnimator.SetFloat("Vertical", dir.y);
        npcHead.SetEyes(dir);
        npcHead.SetHair(dir);
        npcHead.SetHelm(dir);
    }
}
