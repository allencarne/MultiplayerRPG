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

public class ClientJoin : MonoBehaviour
{
    [SerializeField] GameObject playButton;

    private void Start()
    {
        playButton.SetActive(true);
    }

    public async Task FindAndJoinServer()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
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
            ISession session = await MultiplayerService.Instance.JoinSessionByIdAsync(results.Sessions[0].Id);

            // Read the Relay join code stored in session
            string joinCode = session.Properties["joinCode"].Value;
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
}
