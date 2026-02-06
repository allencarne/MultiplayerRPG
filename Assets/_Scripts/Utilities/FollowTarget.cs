using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [HideInInspector] public Transform Target;

    void Update()
    {
        if (Target != null)
        {
            transform.position = Target.position;
        }
    }
}
