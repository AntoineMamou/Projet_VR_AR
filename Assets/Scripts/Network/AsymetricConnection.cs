using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsymmetricConnection : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Scene configuration scriptable object")]
    public SceneConfig sceneConfig;

    [Header("World Spawning")]
    [Tooltip("Reference to WorldSpawner component")]
    public WorldSpawner worldSpawner;

    void Start()
    {
        InitializeNetworking();
    }

    private void InitializeNetworking()
    {
        if (sceneConfig == null)
        {
            Debug.LogError("[RESEAU] SceneConfig non assigné !");
            return;
        }

        // Si on est dans la scene VR, on demarre en tant qu'Hôte (Serveur)
        if (sceneConfig.IsVRScene())
        {
            StartAsHost();
        }
        // Si on est dans la scene AR, on demarre en tant que Client
        else if (sceneConfig.IsARScene())
        {
            StartAsClient();
        }
        else
        {
            Debug.LogWarning($"[RESEAU] Scene actuelle '{sceneConfig.GetCurrentSceneName()}' ne correspond ni à AR ni à VR");
        }
    }

    private void StartAsHost()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("[RESEAU] VR: Serveur Hote demarre avec succes !");
        }
        else
        {
            Debug.LogError("[RESEAU] VR: echec du demarrage du Serveur.");
        }
    }

    private void StartAsClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("[RESEAU] AR: Tentative de connexion au Serveur VR...");
        }
        else
        {
            Debug.LogError("[RESEAU] AR: Impossible de demarrer le Client.");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"[RESEAU] Client connecté: {clientId}");

            // Start spawning objects when client connects
            if (worldSpawner != null)
            {
                worldSpawner.StartSpawning();
            }
            else
            {
                Debug.LogWarning("[RESEAU] Référence WorldSpawner est null !");
            }
        }
        else
        {
            Debug.Log("[RESEAU] Initialisation du serveur local terminée.");
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}