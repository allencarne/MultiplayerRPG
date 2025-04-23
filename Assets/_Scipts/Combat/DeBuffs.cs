using System.Collections;
using UnityEngine;

public class DeBuffs : MonoBehaviour, ISlowable
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    public bool IsSlowed;

    public void Slow(int stacks, float duration)
    {
        StartCoroutine(SlowDuration(stacks, duration));
    }

    IEnumerator SlowDuration(int stacks, float duration)
    {
        IsSlowed = true;
        
        if (player) player.CurrentSpeed = Mathf.Max(0.1f, player.CurrentSpeed - stacks);
        if (enemy) enemy.CurrentSpeed = Mathf.Max(0.1f, enemy.CurrentSpeed - stacks);

        yield return new WaitForSeconds(duration);

        if (player) player.CurrentSpeed += stacks;
        if (enemy) enemy.CurrentSpeed += stacks;

        IsSlowed = false;
    }
}