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
        stats.OnEnemyDamaged.AddListener(TargetAttacker);
        stats.OnDeath.AddListener(DeathState);

        stats.OnDamaged.AddListener(TakeDamage);
        stats.OnDamageDealt.AddListener(DealDamage);

        Invoke("AssignHealth", 1);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnEnemyDamaged.RemoveListener(TargetAttacker);
        stats.OnDeath.RemoveListener(DeathState);

        stats.OnDamaged.RemoveListener(TakeDamage);
        stats.OnDamageDealt.RemoveListener(DealDamage);
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

    private void Update()
    {
        if (InCombat)
        {
            CombatTime += Time.deltaTime;

            if (CombatTime >= 10)
            {
                InCombat = false;
                CombatTime = 0;

                if (stats.net_CurrentHP.Value < stats.net_TotalHP.Value)
                {
                    IsRegen = true;
                    stateMachine.Buffs.regeneration.StartRegen(1, -1);
                }
            }
        }

        if (!IsRegen) return;
        if (stats.net_CurrentHP.Value >= stats.net_TotalHP.Value || InCombat)
        {
            IsRegen = false;
            stateMachine.Buffs.regeneration.StartRegen(-1, -1);
        }
    }

    void AssignHealth()
    {
        if (IsServer)
        {
            stats.net_TotalHP.Value = Data.MaxHealth;
            stats.net_BaseHP.Value = Data.MaxHealth;
            stats.net_CurrentHP.Value = Data.MaxHealth;
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

        if (Data.Items.Length != 0)
        {
            // Shop
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
    }

    void TakeDamage(float damage)
    {
        InCombat = true;
        CombatTime = 0;
    }

    void DealDamage(float damage, Vector2 position)
    {
        InCombat = true;
        CombatTime = 0;
    }
}
