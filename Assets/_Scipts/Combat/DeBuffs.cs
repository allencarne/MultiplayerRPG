using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class DeBuffs : NetworkBehaviour, ISlowable
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;
    int speedBuff = 0;

    public void Slow(int stacks, float duration)
    {
        if (!IsServer) return;

        StartCoroutine(SlowDuration(stacks, duration));
    }

    IEnumerator SlowDuration(int stacks, float duration)
    {
        speedBuff++;
        
        if (player) player.CurrentSpeed.Value = Mathf.Max(0.1f, player.CurrentSpeed.Value - stacks);
        if (enemy) enemy.CurrentSpeed = Mathf.Max(0.1f, enemy.CurrentSpeed - stacks);

        yield return new WaitForSeconds(duration);

        if (player) player.CurrentSpeed.Value += stacks;
        if (enemy) enemy.CurrentSpeed += stacks;

        speedBuff--;
    }
}