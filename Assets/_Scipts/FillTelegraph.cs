using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FillTelegraph : NetworkBehaviour
{
    [SerializeField] SpriteRenderer frontSprite;
    public float FillSpeed;
    [HideInInspector] public CrowdControl crowdControl;
    [HideInInspector] public Enemy enemy;

    private void Update()
    {
        if (!IsServer) return;

        if (crowdControl != null)
        {
            if (crowdControl.IsInterrupted)
            {
                Destroy(gameObject);
            }
        }

        if (enemy != null && enemy.isDead)
        {
            Destroy(gameObject);
            return;
        }

        // Calculate the scale increment per frame to achieve the target scale in FillSpeed seconds
        float scaleIncrement = Time.deltaTime / FillSpeed;

        // Adjust the scale of frontSprite
        Vector3 currentScale = frontSprite.transform.localScale;
        float newScaleX = Mathf.Min(currentScale.x + scaleIncrement, 1f);
        float newScaleY = Mathf.Min(currentScale.y + scaleIncrement, 1f);

        // Set the new scale
        frontSprite.transform.localScale = new Vector3(newScaleX, newScaleY, currentScale.z);

        // Check if the scale has reached 1
        if (frontSprite.transform.localScale.x >= 1f && frontSprite.transform.localScale.y >= 1f)
        {
            Destroy(gameObject);
        }
    }
}