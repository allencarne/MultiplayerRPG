using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PatienceBar : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] Image patienceBarFill;
    public NetworkVariable<float> Patience = new(writePerm: NetworkVariableWritePermission.Server);

    private void OnEnable()
    {
        Patience.OnValueChanged += OnPatienceChanged;
        UpdatePatienceBar(Patience.Value);
    }

    private void OnDisable()
    {
        Patience.OnValueChanged -= OnPatienceChanged;
    }

    private void OnPatienceChanged(float oldValue, float newValue)
    {
        UpdatePatienceBar(newValue);
    }

    public void UpdatePatienceBar(float patience)
    {
        float fillAmount = Mathf.Clamp01(patience / enemy.TotalPatience);
        patienceBarFill.fillAmount = fillAmount;
    }
}
