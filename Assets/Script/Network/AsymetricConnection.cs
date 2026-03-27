using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsymmetricConnection : MonoBehaviour
{
    public GameObject sharedPrefab;

    private void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("[INIT] Scene: " + currentSceneName);

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("[INIT] NetworkManager.Singleton missing.");
            return;
        }

        if (currentSceneName == "VrScene")
        {
            Debug.Log("[HOST] Starting host...");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("[HOST] Host started.");
            }
            else
            {
                Debug.LogError("[HOST] StartHost failed.");
            }
        }
        else if (currentSceneName == "ArScene")
        {
            Debug.Log("[CLIENT] Starting client...");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("[CLIENT] Connection attempt started.");
            }
            else
            {
                Debug.LogError("[CLIENT] StartClient failed.");
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("[NETWORK] Client connected: " + clientId);

        if (NetworkManager.Singleton.IsHost && clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("[HOST] Local host client initialized.");
            return;
        }

        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("[HOST] Remote client connected, loading Level_Design_0.");

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
            NetworkManager.Singleton.SceneManager.OnLoad += OnSceneLoadStarted;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoadComplete;

            SceneEventProgressStatus loadStatus =
                NetworkManager.Singleton.SceneManager.LoadScene("Level_Design_0", LoadSceneMode.Single);

            Debug.Log("[HOST] LoadScene(Level_Design_0) status: " + loadStatus);
        }
    }

    private void OnSceneLoadStarted(
        ulong clientId,
        string sceneName,
        LoadSceneMode loadSceneMode,
        AsyncOperation asyncOperation)
    {
        Debug.Log($"[SCENE] Load start '{sceneName}' for client {clientId} ({loadSceneMode})");
    }

    private void OnSceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        Debug.Log($"[SCENE] Load complete '{sceneName}' for client {clientId} ({loadSceneMode})");
    }

    private void OnSceneLoaded(
        string sceneName,
        LoadSceneMode loadSceneMode,
        List<ulong> clientsCompleted,
        List<ulong> clientsTimedOut)
    {
        Debug.Log(
            $"[SCENE] OnLoadEventCompleted '{sceneName}' complete={clientsCompleted.Count} timeout={clientsTimedOut.Count}");

        if (sceneName == "Level_Design_0" && NetworkManager.Singleton.IsHost)
        {
            Debug.Log("[HOST] Level loaded, spawning shared objects.");
            SpawnSharedObject();

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
            NetworkManager.Singleton.SceneManager.OnLoad -= OnSceneLoadStarted;
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoadComplete;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.LogWarning("[NETWORK] Client disconnected: " + clientId);
    }

    private void SpawnSharedObject()
    {
        if (sharedPrefab == null)
        {
            Debug.Log("[SPAWN] sharedPrefab is null, nothing to spawn.");
            return;
        }

        GameObject obj = Instantiate(sharedPrefab, new Vector3(0, 0, 1), Quaternion.identity);

        if (obj.TryGetComponent<NetworkObject>(out var netObj))
        {
            netObj.Spawn();
            Debug.Log("[SPAWN] Network object spawned successfully.");
        }
        else
        {
            Debug.LogWarning("[SPAWN] sharedPrefab has no NetworkObject.");
        }
    }
}
