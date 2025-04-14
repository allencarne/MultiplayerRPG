using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FillTelegraph : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer frontSprite;
    [SerializeField] private float maxSize;

    public NetworkVariable<float> FillSpeed = new(writePerm: NetworkVariableWritePermission.Server);
    private NetworkVariable<Vector3> CurrentScale = new(Vector3.zero, writePerm: NetworkVariableWritePermission.Server);

    private void OnEnable()
    {
        CurrentScale.OnValueChanged += OnScaleChanged;
        UpdateSpriteScale(CurrentScale.Value);
    }

    private void OnDisable()
    {
        CurrentScale.OnValueChanged -= OnScaleChanged;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(FillRoutine());
        }
        else
        {
            UpdateSpriteScale(CurrentScale.Value);
        }
    }

    private void OnScaleChanged(Vector3 oldScale, Vector3 newScale)
    {
        UpdateSpriteScale(newScale);
    }

    private void UpdateSpriteScale(Vector3 scale)
    {
        frontSprite.transform.localScale = scale;
    }

    IEnumerator FillRoutine()
    {
        while (CurrentScale.Value.x < maxSize || CurrentScale.Value.y < maxSize)
        {
            float increment = Time.deltaTime / FillSpeed.Value;
            Vector3 newScale = CurrentScale.Value;
            newScale.x = Mathf.Min(newScale.x + increment, maxSize);
            newScale.y = Mathf.Min(newScale.y + increment, maxSize);

            CurrentScale.Value = newScale;

            yield return null;
        }

        // Clean up when fill is complete
        GetComponent<NetworkObject>().Despawn();
    }
}