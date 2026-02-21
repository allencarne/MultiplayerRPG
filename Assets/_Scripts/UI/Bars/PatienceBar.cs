using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PatienceBar : NetworkBehaviour
{
    public NetworkVariable<float> Patience = new(writePerm: NetworkVariableWritePermission.Server);
    [SerializeField] Image patienceBarFill;
    [SerializeField] Enemy enemy;
    [SerializeField] NPC npc;

    [Header("Colors")]
    Color lowPatienceColor = Color.yellow;
    Color highPatienceColor = Color.orangeRed;
    Color resettingColor = Color.red;

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
        float totalPatience = 0f;

        if (enemy != null)
        {
            totalPatience = enemy.TotalPatience;
        }

        if (npc != null)
        {
            totalPatience = npc.Data.TotalPatience;
        }

        float fillAmount = Mathf.Clamp01(patience / totalPatience);
        patienceBarFill.fillAmount = fillAmount;

        // If patience is at max (1.0), we're resetting - show reset color
        // Otherwise show gradient from yellow to red
        if (fillAmount >= 0.99f) // Use 0.99 to account for floating point precision
        {
            patienceBarFill.color = resettingColor;
        }
        else
        {
            // Gradient from yellow (0%) to red (100%)
            patienceBarFill.color = Color.Lerp(lowPatienceColor, highPatienceColor, fillAmount);
        }
    }
}