using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SessionManager : MonoBehaviour
{
    NetworkManager networkManager;
    [SerializeField] GameObject playButton;
    [SerializeField] TextMeshProUGUI playerText;

    private Lobby _connectedLobby;
    private QueryResponse _lobbies;
    private UnityTransport _transport;
    private const string JoinCodeKey = "j";
    private string _playerId;
    private bool _hasLeftLobby = false;

    private void Start()
    {
        playButton.SetActive(true);
    }

    private void Awake()
    {
        // Initialize NetworkManager
        networkManager = NetworkManager.Singleton;

        if (networkManager != null)
        {
            // Register the client disconnect callback
            networkManager.OnClientDisconnectCallback += HandleClientDisconnect;
        }
        else
        {
            Debug.LogError("NetworkManager.Singleton is null!");
        }

        // Initialize UnityTransport
        _transport = FindAnyObjectByType<UnityTransport>();
    }

    private void OnDestroy()
    {
        // Unregister the callback to avoid memory leaks
        if (networkManager != null)
        {
            networkManager.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    public async void CreateOrJoinLobby()
    {
        await Authenticate();

        _hasLeftLobby = false;

        _connectedLobby = await QuickJoinLobby() ?? await CreateLobby();

        if (_connectedLobby != null) playButton.SetActive(false);
    }

    private async Task Authenticate()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            var options = new InitializationOptions();
            await UnityServices.InitializeAsync(options);
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            _playerId = AuthenticationService.Instance.PlayerId;
        }
        else
        {
            Debug.Log("Already signed in.");
            _playerId = AuthenticationService.Instance.PlayerId;
        }
    }

    private async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            var a = await RelayService.Instance.JoinAllocationAsync(lobby.Data[JoinCodeKey].Value);

            var relayServerData = AllocationUtils.ToRelayServerData(a, "wss");
            _transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            return lobby;
        }
        catch (Exception e)
        {
            Debug.Log($"No lobbies available via quick join: {e}");
            return null;
        }
    }

    private async Task<Lobby> CreateLobby()
    {
        try
        {
            const int maxPlayers = 100;

            var a = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

            var options = new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> { { JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } }
            };
            var lobby = await LobbyService.Instance.CreateLobbyAsync("Useless Lobby Name", maxPlayers, options);

            StartCoroutine(HeartBeatLobbyCoroutine(lobby.Id, 15));

            var relayServerData = AllocationUtils.ToRelayServerData(a, "wss");
            _transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
            return lobby;
        }
        catch (Exception e)
        {
            Debug.LogFormat($"Failed creating a lobby: {e}");
            return null;
        }
    }

    private static IEnumerator HeartBeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            if (_connectedLobby != null && !_hasLeftLobby)
            {
                _hasLeftLobby = true;
                await LobbyService.Instance.RemovePlayerAsync(_connectedLobby.Id, AuthenticationService.Instance.PlayerId);
            }

            if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
            }

            SceneManager.LoadScene("CharacterSelect");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log($"LeaveLobby error: {e}");
        }
    }

    private async void HandleClientDisconnect(ulong clientId)
    {
        // Log the client ID to the console for debugging
        Debug.Log($"Client with ID {clientId} has disconnected.");

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_connectedLobby.Id, AuthenticationService.Instance.PlayerId);

            if (NetworkManager.Singleton.IsClient)
            {
                Debug.Log("IS CLIENT TRUE");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    void Destroy()
    {
        try
        {
            StopAllCoroutines();

            if (_hasLeftLobby || _connectedLobby == null) return;

            _hasLeftLobby = true;

            if (_connectedLobby.HostId == _playerId)
            {
                LobbyService.Instance.DeleteLobbyAsync(_connectedLobby.Id);
            }
            else
            {
                LobbyService.Instance.RemovePlayerAsync(_connectedLobby.Id, _playerId);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
