using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PatienceBar : NetworkBehaviour
{
    public NetworkVariable<float> Patience = new(writePerm: NetworkVariableWritePermission.Server);
    [SerializeField] Image patienceBarFill;
    [SerializeField] Enemy enemy;
    [SerializeField] NPC npc;

    public override void OnNetworkSpawn()
    {
        Patience.OnValueChanged += OnPatienceChanged;
        UpdatePatienceBar(Patience.Value);
    }

    public override void OnNetworkDespawn()
    {
        Patience.OnValueChanged -= OnPatienceChanged;
    }

    private void OnPatienceChanged(float oldValue, float newValue)
    {
        UpdatePatienceBar(newValue);
    }

    public void UpdatePatienceBar(float patience)
    {
        if (enemy != null)
        {
            float fillAmount = Mathf.Clamp01(patience / enemy.TotalPatience);
            patienceBarFill.fillAmount = fillAmount;
        }

        if (npc != null)
        {
            float fillAmount = Mathf.Clamp01(patience / npc.Data.TotalPatience);
            patienceBarFill.fillAmount = fillAmount;
        }
    }
}