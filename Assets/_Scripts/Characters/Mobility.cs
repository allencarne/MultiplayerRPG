using System.Collections;
using UnityEngine;

public class Mobility : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    public bool IsSliding { get; private set; }
    public bool IsJumping { get; private set; }

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

    public void Jump(Vector2 targetPosition, float duration, float height)
    {
        StartCoroutine(JumpRoutine(targetPosition, duration, height));
    }

    IEnumerator JumpRoutine(Vector2 targetPosition, float duration, float height)
    {
        IsJumping = true;
        Vector2 startPosition = rb.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float movementT = Mathf.SmoothStep(0f, 1f, t);
            Vector2 position = Vector2.Lerp(startPosition,targetPosition,movementT);
            float arc = Mathf.Sin(t * Mathf.PI);
            position.y += arc * height;
            rb.MovePosition(position);
            elapsed += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        rb.MovePosition(targetPosition);
        rb.linearVelocity = Vector2.zero;

        IsJumping = false;
    }

    public void Blink(Vector2 direction, float distance)
    {
        if (direction == Vector2.zero) return;

        Vector2 targetPosition = rb.position + direction.normalized * distance;

        rb.position = targetPosition;
    }
}
