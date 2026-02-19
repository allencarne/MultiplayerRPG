using System.Collections;
using UnityEngine;

public class UIBounceAnimation : MonoBehaviour
{
    [SerializeField] float duration = 0.3f;
    [SerializeField] AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 targetScale;

    private void Awake()
    {
        // Cache whatever scale UIZoom (or anything else) set in its own Awake
        targetScale = transform.localScale;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        transform.localScale = Vector3.zero;
        StartCoroutine(BounceIn());
    }

    private IEnumerator BounceIn()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = bounceCurve.Evaluate(elapsed / duration);
            transform.localScale = targetScale * t;
            yield return null;
        }
        transform.localScale = targetScale;
    }

    public void SetTargetScale(Vector3 scale)
    {
        targetScale = scale;
    }
}
