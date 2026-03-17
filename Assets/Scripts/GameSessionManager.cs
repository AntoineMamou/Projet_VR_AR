using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class GameSessionManager : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] private NetworkManager _networkManager;
    [SerializeField] private bool _limitSessionToTwoPlayers = true;

    [Header("Player Prefabs")]
    [SerializeField] private NetworkObject _arPlayerPrefab;
    [SerializeField] private NetworkObject _vrPlayerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform _arSpawnPoint;
    [SerializeField] private Transform _vrSpawnPoint;

    private readonly Dictionary<ulong, NetworkObject> _spawnedPlayerObjects = new();
    private bool _isConfigured;
    private bool _callbacksRegistered;
    private ulong? _vrClientId;

    private void Awake()
    {
        ConfigureNetworkManager();
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }

    public void ConfigureNetworkManager()
    {
        if (_isConfigured && _callbacksRegistered)
        {
            return;
        }

        if (_networkManager == null)
        {
            _networkManager = NetworkManager.Singleton;
        }

        if (_networkManager == null)
        {
            Debug.LogWarning("[GameSessionManager] NetworkManager is not assigned yet.");
            return;
        }

        _networkManager.NetworkConfig.PlayerPrefab = null;
        _networkManager.NetworkConfig.ConnectionApproval = _limitSessionToTwoPlayers;

        if (_limitSessionToTwoPlayers)
        {
            _networkManager.ConnectionApprovalCallback = ApprovalCheck;
        }

        RegisterCallbacks();
        _isConfigured = true;
    }

    private void RegisterCallbacks()
    {
        if (_networkManager == null || _callbacksRegistered)
        {
            return;
        }

        _networkManager.OnClientConnectedCallback += HandleClientConnected;
        _networkManager.OnClientDisconnectCallback += HandleClientDisconnected;
        _callbacksRegistered = true;
    }

    private void UnregisterCallbacks()
    {
        if (_networkManager == null || !_callbacksRegistered)
        {
            return;
        }

        _networkManager.OnClientConnectedCallback -= HandleClientConnected;
        _networkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
        _callbacksRegistered = false;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        int connectedClientCount = _networkManager.ConnectedClientsIds.Count;
        bool canAcceptClient = !_limitSessionToTwoPlayers || connectedClientCount < 2;

        response.Approved = canAcceptClient;
        response.CreatePlayerObject = false;
        response.Pending = false;

        if (!canAcceptClient)
        {
            response.Reason = "This prototype only supports one AR player and one VR player.";
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (_networkManager == null || !_networkManager.IsServer)
        {
            return;
        }

        if (_spawnedPlayerObjects.ContainsKey(clientId))
        {
            return;
        }

        NetworkObject playerPrefab;
        Transform spawnPoint;

        if (clientId == NetworkManager.ServerClientId)
        {
            playerPrefab = _arPlayerPrefab;
            spawnPoint = _arSpawnPoint;
        }
        else if (!_vrClientId.HasValue)
        {
            _vrClientId = clientId;
            playerPrefab = _vrPlayerPrefab;
            spawnPoint = _vrSpawnPoint;
        }
        else
        {
            Debug.LogWarning($"[GameSessionManager] Client {clientId} connected after the two-player limit was reached.");
            _networkManager.DisconnectClient(clientId);
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("[GameSessionManager] Player prefab is missing.");
            return;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        NetworkObject playerInstance = Instantiate(playerPrefab, spawnPosition, spawnRotation);
        playerInstance.SpawnAsPlayerObject(clientId, true);
        _spawnedPlayerObjects[clientId] = playerInstance;
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        if (_spawnedPlayerObjects.Remove(clientId, out var playerObject) && playerObject != null && playerObject.IsSpawned)
        {
            playerObject.Despawn(true);
        }

        if (_vrClientId == clientId)
        {
            _vrClientId = null;
        }
    }
}
