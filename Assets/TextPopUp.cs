using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TextPopUp : NetworkBehaviour
{
    [SerializeField] GameObject textPopUp;

    public void InstantiatePopUp(float amount)
    {
        if (IsServer)
        {
            PopUpClientRPC(amount);
        }
    }

    [ClientRpc]
    void PopUpClientRPC(float amount)
    {
        Vector2 randomOffset = Random.insideUnitCircle * .3f;
        Vector3 spawnPosition = transform.position + (Vector3)randomOffset;

        GameObject popUp = Instantiate(textPopUp, spawnPosition, Quaternion.identity, transform);
        TextMeshProUGUI popUpText = popUp.GetComponent<TextMeshProUGUI>();
        popUpText.text = amount.ToString();

        Destroy(popUp, 0.5f);
    }
}
