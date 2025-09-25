using System.Collections;
using UnityEngine;

public class CC_Interrupt : MonoBehaviour, IInterruptable
{
    public bool IsInterrupted;

    public void Interrupt()
    {
        StartCoroutine(duration()); ;
    }

    IEnumerator duration()
    {
        IsInterrupted = true;
        yield return new WaitForSeconds(.3f);
        IsInterrupted = false;
    }
}
