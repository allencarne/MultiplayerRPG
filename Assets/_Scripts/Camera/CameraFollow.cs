using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerTransform;
    public float smoothSpeed;

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 desiredPosition = playerTransform.position;
            desiredPosition.z = transform.position.z;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
