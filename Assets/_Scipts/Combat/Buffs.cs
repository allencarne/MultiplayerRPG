using System.Collections;
using UnityEngine;

public class Buffs : MonoBehaviour
{
    public bool IsPhasing;

    public void Phasing(float duration)
    {
        StartCoroutine(PhasingDuration(duration));
    }

    IEnumerator PhasingDuration(float duration)
    {
        IsPhasing = true;

        // Ignore Collision between Player Vs Enemy, Player Vs Player, Enemy Vs Enemy.
        Physics2D.IgnoreLayerCollision(6, 7, true);
        Physics2D.IgnoreLayerCollision(6, 6, true);
        Physics2D.IgnoreLayerCollision(6, 7, true);

        yield return new WaitForSeconds(duration);

        IsPhasing = false;

        // Remove Ignore Collision
        Physics2D.IgnoreLayerCollision(6, 7, false);
        Physics2D.IgnoreLayerCollision(6, 6, false);
        Physics2D.IgnoreLayerCollision(6, 7, false);
    }
}
