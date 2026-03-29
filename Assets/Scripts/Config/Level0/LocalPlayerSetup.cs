using Unity.Netcode;
using UnityEngine;


public class LocalPlayerSetup : NetworkBehaviour
{
    [Header("La Cam�ra attach�e � ce joueur")]
    public Camera playerCamera;

    [Header("L'AudioListener attach� � cette cam�ra (optionnel)")]
    public AudioListener playerAudio;

    public override void OnNetworkSpawn()
    {
 
        if (!IsOwner)
        {
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
