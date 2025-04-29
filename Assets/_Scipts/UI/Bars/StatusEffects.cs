using UnityEngine;
using UnityEngine.UI;

public class StatusEffects : MonoBehaviour
{
    [SerializeField] private Image imageFill;

    private float totalDuration;
    private float remainingDuration;

    public void Initialize(float duration)
    {
        totalDuration = duration;
        remainingDuration = duration;
    }

    private void Update()
    {
        if (remainingDuration > 0)
        {
            remainingDuration -= Time.deltaTime;
            float fillAmount = Mathf.Clamp01(remainingDuration / totalDuration);
            imageFill.fillAmount = fillAmount;
        }
    }
}
