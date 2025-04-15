using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    // Public variable to assign the target position later
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // Set the position of the effect to the target's position
            transform.position = target.position;
        }
    }
}
