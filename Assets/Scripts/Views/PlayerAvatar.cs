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
        // Si cette capsule ne m'appartient pas, je ne fais rien
        if (!IsOwner) return;

        // 1. RECHERCHE DE LA CAMÉRA (Si on ne l'a pas encore trouvée)
        if (cameraLocale == null)
        {
            if (Camera.main != null)
            {
                cameraLocale = Camera.main.transform;
                Debug.Log("[AVATAR] Caméra locale trouvée et assignée !");
            }
            return; // On arrête l'Update ici pour cette frame en attendant de la trouver
        }

        // 2. SUIVI DE LA CAMÉRA (Une fois trouvée)
        transform.position = cameraLocale.position;

        // Optionnel : on copie aussi la rotation (Attention, en AR le téléphone regarde souvent un peu vers le bas)
        transform.rotation = cameraLocale.rotation;
    }
}