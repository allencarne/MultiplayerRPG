using UnityEngine;

public interface IPullable
{
    void Pull(Vector2 direction, float amount, float duration);
}
