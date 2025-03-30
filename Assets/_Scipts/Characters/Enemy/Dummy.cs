using System.Collections;
using UnityEngine;

public class Dummy : Enemy
{
    protected override void IdleState()
    {
        enemyAnimator.Play("Idle");

        if (enemyRB.position != startingPosition)
        {
            idleTime += 1 * Time.deltaTime;
        }

        UpdateDummyPatienceBar();

        if (idleTime >= patience)
        {
            enemyState = EnemyState.Reset;
        }
    }

    protected override void ResetState()
    {
        // Animation
        enemyAnimator.Play("Reset");

        idleTime = 0;

        UpdateDummyPatienceBar();

        StartCoroutine(ResetDuration());
    }

    IEnumerator ResetDuration()
    {
        yield return new WaitForSeconds(.6f);

        enemyRB.linearVelocity = Vector3.zero;
        enemyRB.position = startingPosition;
        enemyState = EnemyState.Spawn;
    }

    void UpdateDummyPatienceBar()
    {
        if (patienceBar != null)
        {
            float fillAmount = Mathf.Clamp01(idleTime / patience);

            patienceBar.fillAmount = fillAmount;
        }
    }
}
