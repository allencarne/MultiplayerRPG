using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TextPopUp : NetworkBehaviour
{
    [SerializeField] GameObject DealDamagePopUp;
    [SerializeField] GameObject TakeDamagePopUp;
    [SerializeField] GameObject HealingPopUp;

    public enum PopUpType
    {
        DealDamage,
        TakeDamage,
        Healing,
    }

    public void DealDamageText(float amount)
    {
        if (IsServer)
        {
            PopUpClientRPC(amount, PopUpType.DealDamage);
        }
    }

    public void TakeDamageText(float amount)
    {
        if (IsServer)
        {
            PopUpClientRPC(amount, PopUpType.TakeDamage);
        }
    }

    public void HealingText(float amount)
    {
        if (IsServer)
        {
            PopUpClientRPC(amount, PopUpType.Healing);
        }
    }

    [ClientRpc]
    void PopUpClientRPC(float amount, PopUpType type)
    {
        Vector2 randomOffset = Random.insideUnitCircle * .7f;
        Vector3 spawnPosition = transform.position + (Vector3)randomOffset;

        GameObject prefab = GetPrefabForType(type);
        if (prefab == null) return;

        GameObject popUp = Instantiate(prefab, spawnPosition, Quaternion.identity, transform);
        TextMeshProUGUI popUpText = popUp.GetComponent<TextMeshProUGUI>();
        popUpText.text = amount.ToString();
    }

    GameObject GetPrefabForType(PopUpType type)
    {
        switch (type)
        {
            case PopUpType.DealDamage: return DealDamagePopUp;
            case PopUpType.TakeDamage: return TakeDamagePopUp;
            case PopUpType.Healing: return HealingPopUp;
            default: return null;
        }
    }
}
