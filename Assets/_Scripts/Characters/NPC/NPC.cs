using Unity.Netcode;
using UnityEngine;

public class NPC : NetworkBehaviour, IInteractable
{
    [Header("Components")]
    [SerializeField] NPCStateMachine stateMachine;
    public NPCData Data;
    public CharacterStats stats;
    [SerializeField] NPCQuest npcQuest;
    [SerializeField] NPCDialogue npcDialogue;

    [Header("UI")]
    public PatienceBar PatienceBar;
    public CastBar CastBar;

    [Header("Variables")]
    public string DisplayName => Data.NPCName;
    public bool IsDead;

    [Header("Sprites")]
    public SpriteRenderer SwordSprite;
    public SpriteRenderer BodySprite;
    public SpriteRenderer HairSprite;
    public SpriteRenderer EyeSprite;
    public SpriteRenderer ShadowSprite;

    public override void OnNetworkSpawn()
    {
        stats.OnEnemyDamaged.AddListener(TargetAttacker);
        stats.OnDeath.AddListener(DeathState);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnEnemyDamaged.RemoveListener(TargetAttacker);
        stats.OnDeath.RemoveListener(DeathState);
    }

    private void Start()
    {
        BodySprite.color = Data.skinColor;
        HairSprite.color = Data.hairColor;

        stats.Speed = Data.Speed;
        stats.Damage = Data.Damage;
        stats.AttackSpeed = Data.AttackSpeed;
        stats.CoolDownReduction = Data.CoolDownRecution;
        stats.Armor = Data.Armor;

        if (IsServer)
        {
            stats.MaxHealth.Value = Data.MaxHealth;
            stats.Health.Value = stats.MaxHealth.Value;
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
        }
        else
        {
            // Dialogue
            player.OpenDialogueUI(Data.NPCName, npcDialogue);
        }
        // If we have a shop

        // If we don't have a quest
        //
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
}
