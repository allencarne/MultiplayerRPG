using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Image healthBar;
    [SerializeField] Image healthBar_Back;

    private float lerpSpeed = 5f;
    private Coroutine lerpCoroutine;

    private NetworkVariable<float> net_health = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> net_maxHealth = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);

    private void Start()
    {
        UpdateHealthUI(player.MaxHealth, player.Health);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            player.TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            player.HealPlayer(1);
        }
    }

    public override void OnNetworkSpawn()
    {
        net_health.OnValueChanged += OnHealthChanged;
        net_maxHealth.OnValueChanged += OnMaxHealthChanged;

        if (IsOwner)
        {
            // Initialize network variable
            net_health.Value = player.Health;
            net_maxHealth.Value = player.MaxHealth;
        }

        // Sync UI for non-owners
        UpdateHealthUI(net_maxHealth.Value, net_health.Value);
    }

    public override void OnDestroy()
    {
        net_health.OnValueChanged -= OnHealthChanged;
        net_maxHealth.OnValueChanged -= OnMaxHealthChanged;
    }

    public void UpdateHealthUI(float maxHealth, float currentHealth)
    {
        if (maxHealth <= 0) return;
        healthBar.fillAmount = currentHealth / maxHealth;

        // Back health bar - smooth lerp effect
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(LerpHealthBarBack(currentHealth / maxHealth));
    }

    IEnumerator LerpHealthBarBack(float targetFillAmount)
    {
        float currentFillAmount = healthBar_Back.fillAmount;

        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
            healthBar_Back.fillAmount = currentFillAmount;
            yield return null;
        }
    }

    public void UpdateHealth(float newHealth)
    {
        if (IsOwner)
        {
            net_health.Value = newHealth;
            UpdateHealthUI(net_maxHealth.Value, newHealth);
        }
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        UpdateHealthUI(net_maxHealth.Value, newValue);
    }

    private void OnMaxHealthChanged(float oldValue, float newValue)
    {
        UpdateHealthUI(newValue, net_health.Value);
    }
}
