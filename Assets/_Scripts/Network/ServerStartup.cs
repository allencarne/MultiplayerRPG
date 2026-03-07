using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ServerStartup : MonoBehaviour
{
    [SerializeField] private int maxConnections = 10;
    [SerializeField] private string sessionName = "MyRPGServer";

    private async void Start()
    {
        if (!Application.isBatchMode) return;

        Debug.Log("=== Dedicated Server Starting ===");
        await StartDedicatedServer();
    }

    private async Task StartDedicatedServer()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in.");

            // Create Relay allocation using WSS for WebGL clients
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log($"=== Relay Join Code: {joinCode} ===");

            // Configure transport with WSS
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            RelayServerData relayData = AllocationUtils.ToRelayServerData(allocation, "wss");
            transport.SetRelayServerData(relayData);

            // Create Session and store join code inside it
            SessionOptions options = new SessionOptions
            {
                Name = sessionName,
                MaxPlayers = maxConnections,
                IsPrivate = false
            };
            options.SessionProperties["joinCode"] = new SessionProperty(joinCode, VisibilityPropertyOptions.Member, PropertyIndex.None);

            IHostSession session = await MultiplayerService.Instance.CreateSessionAsync(options);
            Debug.Log($"Session created: {session.Id}");

            // Start server
            NetworkManager.Singleton.StartServer();
            Debug.Log("=== Server is LIVE ===");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Server startup failed: {e.Message}");
        }
    }
}
