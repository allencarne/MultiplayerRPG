using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        //SceneManager.LoadScene("Garden", LoadSceneMode.Additive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            GetComponent<RuntimeNetStatsMonitor>().Visible = true;
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            GetComponent<RuntimeNetStatsMonitor>().Visible = false;
        }
    }
}
