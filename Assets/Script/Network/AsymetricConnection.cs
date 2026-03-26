using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AsymmetricConnection : MonoBehaviour
{

    public GameObject sharedPrefab;
    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Si on est dans la scène VR, on démarre en tant qu'Hôte (Serveur)
        if (currentSceneName == "VrScene")
        {

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("VR: Serveur Hôte démarré avec succès !");
                
            }
            else
            {
                Debug.LogError("VR: Échec du démarrage du Serveur.");
            }
        }
        // Si on est dans la scène AR, on démarre en tant que Client
        else if (currentSceneName == "ArScene")
        {
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("AR: Tentative de connexion au Serveur VR...");
            }
            else
            {
                Debug.LogError("AR: Impossible de démarrer le Client.");
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            SpawnSharedObject();
        }
        else
        {
            Debug.Log("[RESEAU] C'est juste moi (le Serveur) qui m'initialise.");
        }
    }

    void SpawnSharedObject()
    {
        if (sharedPrefab != null)
        {
            Debug.Log("[SPAWN] Instanciation du cube dans la scène VR");
            GameObject spawnedObject = Instantiate(sharedPrefab, new Vector3(0, 0, 1), Quaternion.identity);

            Debug.Log("[SPAWN] Synchronisation du cube sur le réseau pour l'AR");
            spawnedObject.GetComponent<NetworkObject>().Spawn();

            Debug.Log("[SPAWN] Succès");
        }
        else
        {
            Debug.LogError("[SPAWN ERROR] Le Prefab n'est pas assigné dans l'inspecteur");
        }
    }

}