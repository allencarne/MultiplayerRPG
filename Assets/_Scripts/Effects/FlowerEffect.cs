using System.Collections;
using UnityEngine;

public class FlowerEffect : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] HasteOnTrigger haste;

    private void OnEnable()
    {
        if (haste != null) haste.OnCoolDownStarted.AddListener(Effect);
    }

    private void OnDisable()
    {
        if (haste != null) haste.OnCoolDownStarted.RemoveListener(Effect);
    }

    void Effect(float time)
    {
        StartCoroutine(Duration(time));
    }

    IEnumerator Duration(float time)
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(time);
        sprite.color = Color.white;
    }
}
