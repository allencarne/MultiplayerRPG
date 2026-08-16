using System.Collections;
using UnityEngine;

public class Mobility : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    public bool IsSliding { get; private set; }

    public void Slide(Vector2 direction, float force, float duration)
    {
        if (direction == Vector2.zero) return;
        StartCoroutine(SlideRoutine(direction, force, duration));
    }

    IEnumerator SlideRoutine(Vector2 direction, float force, float duration)
    {
        IsSliding = true;
        float elapsed = 0f;
        Vector2 startVelocity = direction.normalized * force;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            rb.linearVelocity = Vector2.Lerp(startVelocity, Vector2.zero, t);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector2.zero;
        IsSliding = false;
    }
}
