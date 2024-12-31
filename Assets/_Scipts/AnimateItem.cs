using UnityEngine;

public class AnimateItem : MonoBehaviour
{
    [Tooltip("The height of the bobbing motion.")]
    public float bobHeight = 0.2f;

    [Tooltip("The speed of the bobbing motion (higher = slower bobbing).")]
    public float bobSpeed = 1.5f;

    // Initial position of the object
    private Vector3 startPosition;
    private float elapsedTime;

    void Start()
    {
        // Store the starting position of the object
        startPosition = transform.position;

        // Start invoking the bobbing motion
        InvokeRepeating(nameof(UpdateBobbing), 0f, 0.02f);
    }

    void UpdateBobbing()
    {
        // Increment elapsed time
        elapsedTime += 0.02f * bobSpeed;

        // Calculate the new Y position using a sine wave
        float newY = startPosition.y + Mathf.Sin(elapsedTime) * bobHeight;

        // Apply the new position
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
