using System.Collections;
using UnityEngine;

public class CC_Interrupt : MonoBehaviour, IInterruptable
{
    public bool CanInterrupt;

    [SerializeField] PlayerStateMachine player;
    [SerializeField] EnemyStateMachine enemy;
    [SerializeField] NPCStateMachine npc;

    public void Interrupt()
    {
        StartCoroutine(duration());
    }

    IEnumerator duration()
    {
        CanInterrupt = true;

        if (player != null)
        {

        }

        if (enemy != null && enemy.currentAbility != null)
        {
            if (enemy.currentAbility.currentState == EnemyAbility.State.Cast)
            {
                enemy.InterruptAbility(false);
            }
        }

        if (npc != null)
        {

        }

        yield return new WaitForSeconds(.3f);
        CanInterrupt = false;
    }
}
