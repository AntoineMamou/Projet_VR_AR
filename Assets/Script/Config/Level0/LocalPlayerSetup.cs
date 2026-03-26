using Unity.Netcode;
using UnityEngine;

// Attention : On h�rite bien de NetworkBehaviour et pas de MonoBehaviour !
public class LocalPlayerSetup : NetworkBehaviour
{
    [Header("La Cam�ra attach�e � ce joueur")]
    public Camera playerCamera;

    [Header("L'AudioListener attach� � cette cam�ra (optionnel)")]
    public AudioListener playerAudio;

    public override void OnNetworkSpawn()
    {
        // Si ce personnage NE m'appartient PAS (c'est l'avatar de l'autre joueur sur mon �cran)
        if (!IsOwner)
        {
            // Je d�sactive SA cam�ra pour ne pas voir � travers ses yeux
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
            // Si ce personnage m'appartient, je m'assure que ma cam�ra est bien allum�e
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
