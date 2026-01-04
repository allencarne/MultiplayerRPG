using System.Collections;
using UnityEngine;

public class FlowerEffect : MonoBehaviour
{
    [SerializeField] GameObject particle;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] HasteOnTrigger haste;

    [Header("Cooldown Visual Settings")]
    [SerializeField] float shrinkScale = 0.7f;
    [SerializeField] Color cooldownColor;
    [SerializeField] float transitionSpeed = 0.15f;

    private Vector3 originalScale;
    private Color originalColor;

    private void OnEnable()
    {
        if (haste != null) haste.OnCoolDownStarted.AddListener(Effect);
    }

    private void OnDisable()
    {
        if (haste != null) haste.OnCoolDownStarted.RemoveListener(Effect);
    }

    private void Start()
    {
        originalScale = transform.localScale;
        originalColor = sprite.color;
    }

    void Effect(float time)
    {
        StopAllCoroutines();
        StartCoroutine(CooldownAnimation(time));
        Instantiate(particle, transform.position, Quaternion.identity);
    }

    IEnumerator CooldownAnimation(float cooldownDuration)
    {
        yield return StartCoroutine(AnimateToState(originalScale * shrinkScale, cooldownColor, transitionSpeed));

        yield return new WaitForSeconds(cooldownDuration - transitionSpeed * 2);

        yield return StartCoroutine(AnimateToState(originalScale, originalColor, transitionSpeed));
    }

    IEnumerator AnimateToState(Vector3 targetScale, Color targetColor, float duration)
    {
        Vector3 startScale = transform.localScale;
        Color startColor = sprite.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Smooth ease-in-out curve
            t = t * t * (3f - 2f * t);

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            sprite.color = Color.Lerp(startColor, targetColor, t);

            yield return null;
        }

        transform.localScale = targetScale;
        sprite.color = targetColor;
    }
}
