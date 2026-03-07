using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerBoot : MonoBehaviour
{
    private void Start()
    {
        if (!Application.isBatchMode) return;

        Debug.Log("=== Server detected, loading game scene ===");
        SceneManager.LoadScene("Beach");
    }
}
