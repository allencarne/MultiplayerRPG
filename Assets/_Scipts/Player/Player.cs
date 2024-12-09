using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Rigidbody2D rb;
    Vector2 movement;

    [SerializeField] Animator hairAnimator;


    public int hairIndex;
    public int hairColorIndex;
    public int eyeColorIndex;

    private void Start()
    {
        // Get Player Prefs Value and set hairIndex, hairColorIndex, & eyeColorIndex

        if (hairIndex == -1)
        {
            hairAnimator.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
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
