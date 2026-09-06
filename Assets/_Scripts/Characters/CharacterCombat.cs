using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CharacterCombat : NetworkBehaviour
{
    [Header("Combat")]
    [SerializeField] float combatDuration = 10f;

    [Header("Events")]
    public UnityEvent<bool> OnCombatStateChanged;

    public NetworkVariable<bool> InCombat = new(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    Coroutine combatTimerCoroutine;

    public override void OnNetworkSpawn()
    {
        InCombat.OnValueChanged += HandleCombatStateChanged;

        if (IsServer)
        {
            CharacterStats stats = GetComponent<CharacterStats>();

            stats.OnDamaged.AddListener(OnDamaged);
            stats.OnDamageDealt.AddListener(OnDamageDealt);
        }
    }

    public override void OnNetworkDespawn()
    {
        InCombat.OnValueChanged -= HandleCombatStateChanged;

        CharacterStats stats = GetComponent<CharacterStats>();

        stats.OnDamaged.RemoveListener(OnDamaged);
        stats.OnDamageDealt.RemoveListener(OnDamageDealt);

        if (combatTimerCoroutine != null)
        {
            StopCoroutine(combatTimerCoroutine);
        }
    }

    void OnDamaged(float damage)
    {
        EnterCombat();
    }

    void OnDamageDealt()
    {
        EnterCombat();
    }

    void EnterCombat()
    {
        if (!IsServer) return;

        InCombat.Value = true;

        if (combatTimerCoroutine != null)
        {
            StopCoroutine(combatTimerCoroutine);
        }

        combatTimerCoroutine = StartCoroutine(CombatTimer());
    }

    IEnumerator CombatTimer()
    {
        yield return new WaitForSeconds(combatDuration);

        InCombat.Value = false;
        combatTimerCoroutine = null;
    }

    void HandleCombatStateChanged(bool previousValue, bool newValue)
    {
        OnCombatStateChanged?.Invoke(newValue);
    }
}
