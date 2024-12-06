using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody2D rb;
    Vector2 movement;

    private void Update()
    {
        // Only process input if this is the owner
        if (!IsOwner) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movement = new Vector2(moveX, moveY).normalized;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        rb.linearVelocity = movement * moveSpeed;
    }
}
