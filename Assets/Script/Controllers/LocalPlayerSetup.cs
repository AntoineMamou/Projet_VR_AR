using Unity.Netcode;
using UnityEngine;

// Attention : On hérite bien de NetworkBehaviour et pas de MonoBehaviour !
public class LocalPlayerSetup : NetworkBehaviour
{
    [Header("La Caméra attachée à ce joueur")]
    public Camera playerCamera;

    [Header("L'AudioListener attaché à cette caméra (optionnel)")]
    public AudioListener playerAudio;

    public override void OnNetworkSpawn()
    {
        // Si ce personnage NE m'appartient PAS (c'est l'avatar de l'autre joueur sur mon écran)
        if (!IsOwner)
        {
            // Je désactive SA caméra pour ne pas voir à travers ses yeux
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(false);
            }

            if (playerAudio != null)
            {
                playerAudio.enabled = false;
            }
        }
        else
        {
            // Si ce personnage m'appartient, je m'assure que ma caméra est bien allumée
            if (playerCamera != null)
            {
                playerCamera.gameObject.SetActive(true);
            }
            if (playerAudio != null)
            {
                playerAudio.enabled = true;
            }
        }
    }
}