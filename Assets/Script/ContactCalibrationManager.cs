using UnityEngine;
using Unity.XR.CoreUtils;

public class ContactCalibrationManager : MonoBehaviour
{
    [Header("Références Locales")]
    public XROrigin xrOrigin;

    [Header("Références Réseau")]
    [Tooltip("L'objet dans la scène AR qui suit la position réseau du casque VR")]
    private Transform vrHeadsetProxy;

    public void CalibrerParContact()
    {
        if (vrHeadsetProxy == null)
        {
            GameObject vrPlayer = GameObject.FindGameObjectWithTag("Player");

            if (vrPlayer != null)
            {
                vrHeadsetProxy = vrPlayer.transform;
                Debug.Log("[COLOC] Casque VR trouvé dynamiquement !");
            }
            else
            {
                Debug.LogError("[COLOC] Échec : Aucun objet avec le tag 'VRHeadset' n'est dans la scène.");
                return;
            }
        }

        if (xrOrigin == null) return;


        Transform objectToCalibrate = xrOrigin.transform;      // Le monde AR à bouger
        Transform poseToAlign = xrOrigin.Camera.transform;     // Le téléphone (Caméra AR)
        Transform referencePose = vrHeadsetProxy;              // Le casque VR

        Vector3 scaledExpectedLocalPos = Vector3.Scale(poseToAlign.localPosition, objectToCalibrate.localScale);

        // Étape 2 : Rotation avec Quaternions (Et on ajoute les 180° pour le Face-à-Face)
        Quaternion demiTour = Quaternion.Euler(0, 180f, 0);
        Quaternion rotationCible = referencePose.rotation * demiTour;

        objectToCalibrate.rotation = rotationCible * Quaternion.Inverse(poseToAlign.localRotation);

        // Étape 3 : Appliquer la position finale
        Vector3 finalPosition = referencePose.position - objectToCalibrate.rotation * scaledExpectedLocalPos;
        objectToCalibrate.position = finalPosition;

        Debug.Log("[COLOC] Calibration Handshake PRO (Face-à-Face) réussie !");
    }
}