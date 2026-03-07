using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;

public class ClientJoin : MonoBehaviour
{
    [SerializeField] GameObject playButton;
    private ISession _currentSession;

    private void Start()
    {
        playButton.SetActive(true);
    }

    private void OnEnable()
    {
        LeaveSession.OnLeaveButton += LeaveSessionButton;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnected;
    }

    private void OnDisable()
    {
        LeaveSession.OnLeaveButton -= LeaveSessionButton;
        if (NetworkManager.Singleton != null) NetworkManager.Singleton.OnClientDisconnectCallback -= OnDisconnected;
    }

    public async Task FindAndJoinServer()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            Debug.Log("Signed in.");

            // Find available sessions
            QuerySessionsOptions options = new QuerySessionsOptions();
            QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(options);

            if (results.Sessions.Count == 0)
            {
                Debug.LogWarning("No sessions found.");
                return;
            }

            // Join the first available session
            _currentSession = await MultiplayerService.Instance.JoinSessionByIdAsync(results.Sessions[0].Id);

            // Read the Relay join code stored in session
            string joinCode = _currentSession.Properties["joinCode"].Value;
            Debug.Log($"Got join code: {joinCode}");

            // Join Relay using WSS for WebGL
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayData = AllocationUtils.ToRelayServerData(joinAllocation, "wss");
            transport.SetRelayServerData(relayData);

            NetworkManager.Singleton.StartClient();
            Debug.Log("=== Client connected! ===");

            playButton.SetActive(false);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Client join failed: {e.Message}");
        }
    }

    public async void OnFindGameButtonClicked()
    {
        await FindAndJoinServer();
    }

    async void LeaveSessionButton()
    {
        if (_currentSession != null)
        {
            try { await _currentSession.LeaveAsync(); }
            catch { }
            finally { _currentSession = null; }
        }

        if (NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene("CharacterSelect");
    }

    private async void OnDisconnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        Debug.LogWarning("Disconnected from server.");

        if (_currentSession != null)
        {
            try { await _currentSession.LeaveAsync(); }
            catch { }
            finally { _currentSession = null; }
        }

        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("CharacterSelect");
    }
}
