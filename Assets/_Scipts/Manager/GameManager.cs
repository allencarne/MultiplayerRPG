using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("Garden", LoadSceneMode.Additive);
    }
}
