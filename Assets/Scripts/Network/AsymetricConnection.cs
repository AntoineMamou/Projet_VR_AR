using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsymmetricConnection : MonoBehaviour
{
    public GameObject sharedPrefab;

    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("[INIT] Scene: " + currentSceneName);

        //casque = HOST
        if (currentSceneName == "VrScene")
        {
            Debug.Log("[HOST] Démarrage du Host...");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            if (NetworkManager.Singleton.StartHost())
                Debug.Log("[HOST] Host démarré !");
            else
                Debug.LogError("[HOST] Échec du Host !");
        }
        //tel = CLIENT
        else if (currentSceneName == "ArScene")
        {
            Debug.Log("[CLIENT] Attente avant connexion...");
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            if (NetworkManager.Singleton.StartClient())
                Debug.Log("[CLIENT] Tentative de connexion...");
            else
                Debug.LogError("[CLIENT] Échec du StartClient !");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("[RESEAU] Client connecté: " + clientId);

        if (NetworkManager.Singleton.IsHost && clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("[HOST] Initialisation locale");
            return;
        }

        // Seulement le host décide de changer de scène
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("[HOST] Client distant connecté → Lancement du niveau !");

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;

            NetworkManager.Singleton.SceneManager.LoadScene("Level_Design_0", LoadSceneMode.Single);
        }
    }

    // Déclenché uniquement quand tout le monde est arrivé dans la nouvelle scène
    private void OnSceneLoaded(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, System.Collections.Generic.List<ulong> clientsCompleted, System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        if (sceneName == "Level_Design_0" && NetworkManager.Singleton.IsHost)
        {
            Debug.Log("[HOST] Niveau chargé ! Apparition des objets...");
            SpawnSharedObject();

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.LogWarning("[RESEAU] Client déconnecté: " + clientId);
    }

    void SpawnSharedObject()
    {
        if (sharedPrefab == null) return;

        GameObject obj = Instantiate(sharedPrefab, new Vector3(0, 0, 1), Quaternion.identity);

        if (obj.TryGetComponent<NetworkObject>(out var netObj))
        {
            netObj.Spawn();
            Debug.Log("[SPAWN] Objet réseau spawn avec succès !");
        }
    }
}