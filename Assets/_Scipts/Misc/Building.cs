using System.ComponentModel;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] GameObject inside;
    [SerializeField] GameObject outside;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inside.SetActive(true);
            outside.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inside.SetActive(false);
            outside.SetActive(true);
        }
    }
}
