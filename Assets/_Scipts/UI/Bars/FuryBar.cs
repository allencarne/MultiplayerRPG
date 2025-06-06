using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FuryBar : MonoBehaviour
{
    [SerializeField] Image furyBar;
    [SerializeField] Image furyBar_Back;

    private float lerpSpeed = 5f;
    private Coroutine lerpCoroutine;

    public void UpdateFuryBar(float maxFury, float currentFury)
    {
        if (maxFury <= 0) return;

        furyBar_Back.fillAmount = currentFury / maxFury;

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(LerpHealthBarBack(currentFury / maxFury));
    }

    IEnumerator LerpHealthBarBack(float targetFillAmount)
    {
        float currentFillAmount = furyBar.fillAmount;

        while (!Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
            furyBar.fillAmount = currentFillAmount;
            yield return null;
        }
    }
}
