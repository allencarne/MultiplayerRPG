using System.Collections;
using UnityEngine;

public class CC_Interrupt : MonoBehaviour, IInterruptable
{
    [SerializeField] PlayerStateMachine player;
    [SerializeField] EnemyStateMachine enemy;
    [SerializeField] NPCStateMachine npc;

    public void Interrupt()
    {
        if (player != null) player.Interrupt();
        if (enemy != null) enemy.Interrupt();
        if (npc != null) npc.Interrupt();
    }
}
