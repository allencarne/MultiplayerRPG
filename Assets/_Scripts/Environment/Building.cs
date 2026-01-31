using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] GameObject inside;
    [SerializeField] GameObject outside;
    [SerializeField] GameObject sign;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 playerPos = collision.transform.position;

            if (playerPos.y > transform.position.y)
            {
                inside.SetActive(true);
                outside.SetActive(false);
                sign.SetActive(false);
            }

            if (playerPos.y < transform.position.y)
            {
                inside.SetActive(false);
                outside.SetActive(true);
                sign.SetActive(true);
            }
        }
    }
}
