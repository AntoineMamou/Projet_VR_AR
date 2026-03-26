using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsymmetricConnection : MonoBehaviour
{
    public GameObject sharedPrefab;

    IEnumerator Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        Debug.Log("[INIT] Scene: " + currentSceneName);

        // 🔵 TELEPHONE = HOST
        if (currentSceneName == "ArScene")
        {
            Debug.Log("[HOST] Démarrage du Host...");

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            if (NetworkManager.Singleton.StartHost())
                Debug.Log("[HOST] Host démarré !");
            else
                Debug.LogError("[HOST] Échec du Host !");
        }

        // 🟣 CASQUE = CLIENT
        else if (currentSceneName == "VrScene")
        {
            Debug.Log("[CLIENT] Attente avant connexion...");

            // 🔥 IMPORTANT : laisse le temps au host de démarrer
            yield return new WaitForSeconds(2f);

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            if (NetworkManager.Singleton.StartClient())
                Debug.Log("[CLIENT] Tentative de connexion...");
            else
                Debug.LogError("[CLIENT] Échec du StartClient !");
        }
    }

    // ✅ Quand un client se connecte
    private void OnClientConnected(ulong clientId)
    {
        Debug.Log("[RESEAU] Client connecté: " + clientId);

        // Ignore la connexion locale du host
        if (NetworkManager.Singleton.IsHost &&
            clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("[HOST] Initialisation locale");
            return;
        }

        // Seulement le host exécute ça
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("[HOST] Client distant connecté → spawn + changement de scène");

            SpawnSharedObject();

            NetworkManager.Singleton.SceneManager.LoadScene(
                "Level_Design_0",
                LoadSceneMode.Single
            );
        }
    }

    // ❌ Si déconnexion
    private void OnClientDisconnected(ulong clientId)
    {
        Debug.LogWarning("[RESEAU] Client déconnecté: " + clientId);
    }

    // 📦 Spawn objet réseau
    void SpawnSharedObject()
    {
        if (sharedPrefab == null)
        {
            Debug.LogError("[SPAWN ERROR] Prefab non assigné !");
            return;
        }

        GameObject obj = Instantiate(sharedPrefab, new Vector3(0, 0, 1), Quaternion.identity);

        if (!obj.TryGetComponent<NetworkObject>(out var netObj))
        {
            Debug.LogError("[SPAWN ERROR] Pas de NetworkObject sur le prefab !");
            return;
        }

        netObj.Spawn();

        Debug.Log("[SPAWN] Objet réseau spawn avec succès !");
    }
}