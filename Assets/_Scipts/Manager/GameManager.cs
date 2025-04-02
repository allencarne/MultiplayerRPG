using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("Garden", LoadSceneMode.Additive);
    }
}
