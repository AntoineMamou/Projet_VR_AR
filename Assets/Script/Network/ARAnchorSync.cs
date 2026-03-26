using Unity.Netcode;
using UnityEngine;
using Unity.XR.CoreUtils;
public class ARAnchorSync : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsClient && !IsHost)
        {
            XROrigin xrOrigin = FindAnyObjectByType<XROrigin>();

            if (xrOrigin != null)
            {
                // On accroche visuellement le cube à l'espace AR !
                transform.SetParent(xrOrigin.transform, false);
                Debug.Log("[AR] Le cube réseau s'est bien accroché à l'XR Origin !");
            }
            else
            {
                Debug.LogWarning("[AR] Impossible de trouver l'XR Origin...");
            }
        }
    }
}