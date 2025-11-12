using UnityEngine;

public class TravelToTarget : MonoBehaviour
{
    public Transform target;
    float speed = 0.2f;

    private void Update()
    {
        if (target == null) return;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Destroy(gameObject);
    }
}
