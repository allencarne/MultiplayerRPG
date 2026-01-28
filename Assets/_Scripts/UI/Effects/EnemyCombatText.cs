using TMPro;
using Unity.Netcode;
using UnityEngine;

public class EnemyCombatText : NetworkBehaviour
{
    [SerializeField] CharacterStats stats;
    [SerializeField] GameObject Deal_Prefab;

    [SerializeField] RectTransform hightRect;
    [SerializeField] RectTransform lowRect;

    public override void OnNetworkSpawn()
    {
        stats.OnDamaged.AddListener(HurtClientRPC);
    }

    public override void OnNetworkDespawn()
    {
        stats.OnDamaged.RemoveListener(HurtClientRPC);
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
}
