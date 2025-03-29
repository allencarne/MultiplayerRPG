using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class EnduranceBar : NetworkBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Image enduranceBar;
    bool isRecharging = false;

    private NetworkVariable<float> net_endurance = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> net_maxEndurance = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);

    private void Start()
    {
        UpdateEnduranceUI(player.MaxEndurance, player.Endurance);
    }

    public override void OnNetworkSpawn()
    {
        net_endurance.OnValueChanged += OnEnduranceChanged;
        net_maxEndurance.OnValueChanged += OnMaxEnduranceChanged;

        if (IsOwner)
        {
            // Initialize endurance network variable
            net_endurance.Value = player.Endurance;
            net_maxEndurance.Value = player.MaxEndurance;
        }

        // Sync UI for non-owners
        UpdateEnduranceUI(net_maxEndurance.Value, net_endurance.Value);
    }

    public override void OnDestroy()
    {
        net_endurance.OnValueChanged -= OnEnduranceChanged;
        net_maxEndurance.OnValueChanged -= OnMaxEnduranceChanged;
    }

    void UpdateEnduranceUI(float maxEndurance, float currentEndurance)
    {
        if (maxEndurance <= 0) return;
        enduranceBar.fillAmount = currentEndurance / maxEndurance;
    }

    public void UpdateEndurance(float amount)
    {
        if (IsOwner)
        {
            player.Endurance -= amount;

            net_endurance.Value = player.Endurance;
            UpdateEnduranceUI(net_maxEndurance.Value, player.Endurance);

            if (!isRecharging)
            {
                StartCoroutine(RechargeEndurance());
            }
        }
    }

    IEnumerator RechargeEndurance()
    {
        isRecharging = true;

        while (player.Endurance < net_maxEndurance.Value)
        {
            yield return new WaitForSeconds(1);

            if (IsOwner)
            {
                player.Endurance += 5;
                player.Endurance = Mathf.Min(player.Endurance, net_maxEndurance.Value);

                net_endurance.Value = player.Endurance;
                UpdateEnduranceUI(net_maxEndurance.Value, player.Endurance);
            }
        }

        isRecharging = false;
    }

    private void OnEnduranceChanged(float oldValue, float newValue)
    {
        UpdateEnduranceUI(net_maxEndurance.Value, newValue);
    }

    private void OnMaxEnduranceChanged(float oldValue, float newValue)
    {
        UpdateEnduranceUI(newValue, net_endurance.Value);
    }
}
