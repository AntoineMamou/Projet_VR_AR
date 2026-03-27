using Unity.Netcode;
using UnityEngine;

public class LocalPlayerSetup : NetworkBehaviour
{
    [Header("La camera attachee a ce joueur")]
    public Camera playerCamera;

    [Header("L'AudioListener attache a cette camera (optionnel)")]
    public AudioListener playerAudio;

    private VictoryPanelController _victoryPanelController;

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

            return;
        }

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }

        if (playerAudio != null)
        {
            playerAudio.enabled = true;
        }

        EnsureVictoryPanelController();
    }

    private void EnsureVictoryPanelController()
    {
        if (playerCamera == null)
        {
            return;
        }

        if (_victoryPanelController == null)
        {
            _victoryPanelController = GetComponent<VictoryPanelController>();
        }

        if (_victoryPanelController == null)
        {
            _victoryPanelController = gameObject.AddComponent<VictoryPanelController>();
        }

        _victoryPanelController.Initialize(playerCamera);
    }
}
