using Unity.Netcode;
using UnityEngine;

public class PlayerAvatar : NetworkBehaviour // Attention, héritage de NetworkBehaviour !
{
    private Transform cameraLocale;

    void Start()
    {
        // On vérifie si CETTE capsule m'appartient (IsOwner)
        if (IsOwner)
        {
            // Je cherche la caméra principale de ma scène (Casque VR ou Téléphone AR)
            if (Camera.main != null)
            {
                cameraLocale = Camera.main.transform;
            }
        }
    }

    void Update()
    {
        // Si c'est mon avatar, je force ma capsule à suivre MA caméra
        if (IsOwner && cameraLocale != null)
        {
            transform.position = cameraLocale.position;

            // Optionnel : on copie aussi la rotation pour voir où le joueur regarde
            transform.rotation = cameraLocale.rotation;
        }
    }
}