using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Buffs : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] Enemy enemy;

    [SerializeField] GameObject buffBar;
    [SerializeField] GameObject buff_Phasing;
    [SerializeField] GameObject buff_Immune;
    [SerializeField] GameObject buff_Immovable;
    [SerializeField] GameObject buff_Haste;

    GameObject hasteInstance;

    public bool IsPhasing;
    public bool IsImmune;
    public bool IsImmovable;

    int hasteStacks = 0;

    #region Phasing

    public void Phasing(float duration)
    {
        StartCoroutine(PhasingDuration(duration));
    }

    IEnumerator PhasingDuration(float duration)
    {
        IsPhasing = true;

        GameObject buff = Instantiate(buff_Phasing, buffBar.transform);

        // Ignore Collision between Player Vs Enemy, Player Vs Player, Enemy Vs Enemy.
        Physics2D.IgnoreLayerCollision(6, 7, true);
        Physics2D.IgnoreLayerCollision(6, 6, true);
        Physics2D.IgnoreLayerCollision(6, 7, true);

        yield return new WaitForSeconds(duration);

        IsPhasing = false;

        Destroy(buff);

        // Remove Ignore Collision
        Physics2D.IgnoreLayerCollision(6, 7, false);
        Physics2D.IgnoreLayerCollision(6, 6, false);
        Physics2D.IgnoreLayerCollision(6, 7, false);
    }
    #endregion

    #region Immunity

    public void Immunity(float duration)
    {
        StartCoroutine(ImmunityDuration(duration));
    }

    IEnumerator ImmunityDuration(float duration)
    {
        GameObject buff = Instantiate(buff_Immune, buffBar.transform);

        IsImmune = true;

        yield return new WaitForSeconds(duration);

        Destroy(buff);

        IsImmune = false;
    }

    #endregion

    #region Immovable

    public void Immoveable(float duration)
    {
        StartCoroutine(ImmovableDuration(duration));
    }

    IEnumerator ImmovableDuration(float duration)
    {
        GameObject buff = Instantiate(buff_Immovable, buffBar.transform);

        IsImmovable = true;

        yield return new WaitForSeconds(duration);

        Destroy(buff);

        IsImmovable = false;
    }

    #endregion

    public void Haste(int stacks, float duration)
    {
        StartCoroutine(HasteDuration(stacks, duration));
    }

    IEnumerator HasteDuration(int stacks, float duration)
    {
        hasteStacks += stacks;
        hasteStacks = Mathf.Min(hasteStacks, 25);

        if (!hasteInstance)
        {
            hasteInstance = Instantiate(buff_Haste, buffBar.transform);
        }

        hasteInstance.GetComponentInChildren<TextMeshProUGUI>().text = hasteStacks.ToString();

        // Apply Haste
        if (player != null) player.CurrentSpeed.Value = player.BaseSpeed.Value + hasteStacks;
        if (enemy != null) enemy.CurrentSpeed = enemy.BaseSpeed + hasteStacks;

        yield return new WaitForSeconds(duration);

        hasteStacks -= stacks;
        hasteStacks = Mathf.Max(hasteStacks, 0);

        // Apply Haste
        if (player != null) player.CurrentSpeed.Value = player.BaseSpeed.Value + hasteStacks;
        if (enemy != null) enemy.CurrentSpeed = enemy.BaseSpeed + hasteStacks;

        if (hasteStacks == 0)
        {
            Destroy(hasteInstance);
        }

        hasteInstance.GetComponentInChildren<TextMeshProUGUI>().text = hasteStacks.ToString();
    }
}
