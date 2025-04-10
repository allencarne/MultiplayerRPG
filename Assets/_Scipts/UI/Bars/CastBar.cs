using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CastBar : MonoBehaviour
{
    [SerializeField] private Image castBar;
    private Coroutine currentCastCoroutine;

    public void StartCast(float castTime, float attackSpeed)
    {
        float modifiedCastTime = castTime / attackSpeed;

        // Stop any previous cast in progress
        if (currentCastCoroutine != null)
        {
            StopCoroutine(currentCastCoroutine);
        }

        currentCastCoroutine = StartCoroutine(HandleCast(modifiedCastTime));
    }

    private IEnumerator HandleCast(float duration)
    {
        float elapsed = 0f;
        castBar.fillAmount = 0f;
        castBar.color = Color.black;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            castBar.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        castBar.fillAmount = 1f;

        // Optional: reset color or trigger some completion logic
        castBar.color = Color.green;
    }

    public void StartRecovery(float recoveryTime)
    {
        if (currentCastCoroutine != null)
        {
            StopCoroutine(currentCastCoroutine);
        }

        currentCastCoroutine = StartCoroutine(HandleRecovery(recoveryTime));
    }

    IEnumerator HandleRecovery(float duration)
    {
        float elapsed = 0f;
        castBar.fillAmount = 0f;
        castBar.color = Color.yellow;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            castBar.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        castBar.fillAmount = 0f;
    }

    public void CastBarInterrupted()
    {
        // Assign Color
        castBar.color = Color.red;

        // Full Cast Bar
        castBar.fillAmount = 1;

        // Reset
        StartCoroutine(ResetCastBar());
    }

    IEnumerator ResetCastBar()
    {
        yield return new WaitForSeconds(.2f);

        castBar.fillAmount = 0;
    }
}
