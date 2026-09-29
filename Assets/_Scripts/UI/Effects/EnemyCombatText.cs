using TMPro;
using Unity.Netcode;
using UnityEngine;

public class EnemyCombatText : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] GameObject Deal_Prefab;
    [SerializeField] GameObject HurtCrit_Prefab;

    [SerializeField] RectTransform hightRect;
    [SerializeField] RectTransform lowRect;

    public override void OnNetworkSpawn()
    {
        stats.OnDamaged.AddListener(HurtClientRPC);
        stats.OnCritTaken.AddListener(CritTaken);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDamaged.RemoveListener(HurtClientRPC);
        stats.OnCritTaken.RemoveListener(CritTaken);
    }

    [ClientRpc]
    void HurtClientRPC(float amount)
    {
        Vector2 randomOffset = Random.insideUnitCircle * .7f;
        Vector2 spawnPosition = (Vector2)hightRect.transform.position + randomOffset;

        GameObject popUp = Instantiate(Deal_Prefab, spawnPosition, Quaternion.identity, transform);
        TextMeshProUGUI popUpText = popUp.GetComponent<TextMeshProUGUI>();

        popUpText.text = amount.ToString();
    }

    void CritTaken(int amount, NetworkObject attacker)
    {
        // Use head/higher position for crits
        Vector2 spawnPos = (Vector2)hightRect.transform.position;
        if (IsServer)
        {
            CritClientRPC(amount, spawnPos);
        }
        else
        {
            CritServerRPC(amount, spawnPos);
        }
    }

    [ClientRpc]
    void CritClientRPC(int amount, Vector2 spawnPosition)
    {
        Vector2 randomOffset = Random.insideUnitCircle * .7f;
        Vector2 finalPos = spawnPosition + randomOffset;

        if (HurtCrit_Prefab == null) return;
        GameObject popUp = Instantiate(HurtCrit_Prefab, finalPos, Quaternion.identity, transform);
        TextMeshProUGUI popUpText = popUp.GetComponent<TextMeshProUGUI>();
        if (popUpText != null) popUpText.text = amount.ToString();
    }

    [ServerRpc]
    void CritServerRPC(int amount, Vector2 spawnPosition)
    {
        CritClientRPC(amount, spawnPosition);
    }
}
