using Unity.Netcode;
using UnityEngine;

public class NPC : NetworkBehaviour, IDamageable, IHealable, IInteractable
{
    public NPCData Data;
    public CharacterStats stats;
    public string DisplayName => Data.NPCName;

    [Header("Variables")]
    public bool IsDead;

    [Header("Sprites")]
    public SpriteRenderer SwordSprite;
    public SpriteRenderer BodySprite;
    public SpriteRenderer HairSprite;
    public SpriteRenderer EyeSprite;
    public SpriteRenderer ShadowSprite;

    [SerializeField] NPCQuest npcQuest;
    [SerializeField] NPCDialogue npcDialogue;

    [Header("Components")]
    public CastBar CastBar;
    [SerializeField] NPCStateMachine stateMachine;
    [SerializeField] GameObject death_Effect;
    public PatienceBar PatienceBar;
    public GameObject spawn_Effect;

    private void Start()
    {
        BodySprite.color = Data.skinColor;
        HairSprite.color = Data.hairColor;

        stats.Speed = Data.Speed;
        stats.Damage = Data.Damage;
        stats.AttackSpeed = Data.AttackSpeed;
        stats.CoolDownReduction = Data.CoolDownRecution;
        stats.Armor = Data.Armor;
    }

    public override void OnNetworkSpawn()
    {
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

    #region Damage
    public void TakeDamage(float damage, DamageType damageType, NetworkObject attackerID)
    {

    }

    public void GiveHeal(float healAmount, HealType healType)
    {

    }

    #endregion

    #region Death
    [ClientRpc]
    void DeathClientRPC()
    {
        if (!IsOwner) return;
        stateMachine.SetState(NPCStateMachine.State.Death);
        Instantiate(death_Effect, transform.position, transform.rotation);
    }

    #endregion

    #region Target
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

    #endregion
}
