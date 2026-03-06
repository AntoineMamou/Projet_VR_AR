using Unity.Netcode;
using UnityEngine;
using System.Text;

public class ConnectionManager : MonoBehaviour
{

    public GameObject connectionUI;


    public void StartVRHost()
    {

        SetConnectionPayload("VR");

        // Le joueur VR héberge la partie (Host = Serveur + Client)
        NetworkManager.Singleton.StartHost();
        connectionUI.SetActive(false);
    }

    
    public void StartARClient()
    {

        SetConnectionPayload("AR");

  
        NetworkManager.Singleton.StartClient();

        Debug.Log("=== TENTATIVE DE CONNEXION AR ===");

        connectionUI.SetActive(false);
    }

    private void SetConnectionPayload(string role)
    {
        byte[] payloadBytes = Encoding.UTF8.GetBytes(role);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;
    }
}