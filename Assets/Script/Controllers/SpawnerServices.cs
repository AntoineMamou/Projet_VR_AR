using Unity.Netcode;
using UnityEngine;
using System.Text;

public class SpawnerServices : MonoBehaviour
{
    [Header("Assigner les Prefabs depuis l'éditeur")]
    public GameObject vrPlayerPrefab;
    public GameObject arPlayerPrefab;

    private void Start()
    {
        // On s'abonne à l'événement d'approbation de Netcode
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = Encoding.UTF8.GetString(request.Payload);

        Debug.Log($"[SpawnerServices] ---> Nouvelle demande de connexion reçue ! Client ID: {request.ClientNetworkId} | Payload reçu: '{payload}'");

        response.Approved = true;
        response.CreatePlayerObject = true;


        if (payload == "VR")
        {
            Debug.Log($"[SpawnerServices] Configuration du joueur VR en cours...");
            response.PlayerPrefabHash = vrPlayerPrefab.GetComponent<NetworkObject>().PrefabIdHash;
            response.Position = new Vector3(0, 1, 0);
        }
        else if (payload == "AR")
        {

            Debug.Log($"[SpawnerServices] Joueur AR détecté ! Préparation du spawn pour le Client {request.ClientNetworkId}...");

          
            if (arPlayerPrefab == null)
            {
                Debug.LogError("[SpawnerServices] ERREUR FATALE : Le arPlayerPrefab n'est pas assigné dans l'inspecteur de SpawnerServices !");
            }
            else
            {
                response.PlayerPrefabHash = arPlayerPrefab.GetComponent<NetworkObject>().PrefabIdHash;
                Debug.Log($"[SpawnerServices] Succès : Hash du Prefab AR assigné ({response.PlayerPrefabHash}).");
            }

            response.Position = new Vector3(0, 1, 0);
            Debug.Log($"[SpawnerServices] Position de départ du joueur AR fixée à : {response.Position}");
        }
        else
        {
            // Si le message est inconnu, on refuse la connexion par sécurité
            response.Approved = false;
            response.CreatePlayerObject = false;
            Debug.LogWarning($"[SpawnerServices] Connexion refusée : Rôle inconnu ('{payload}').");
        }

     
        Debug.Log($"[SpawnerServices] <--- Décision finale pour le Client {request.ClientNetworkId} : Approuvé = {response.Approved}, Créer l'avatar = {response.CreatePlayerObject}");
    }
}