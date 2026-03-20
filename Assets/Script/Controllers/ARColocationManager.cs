using Unity.Netcode;
using UnityEngine;
using Unity.XR.CoreUtils;

public class ARColocationManager : MonoBehaviour
{
    [Header("Références (À glisser dans l'inspecteur)")]
    public XROrigin xrOrigin;
    public ARMarkerScanner arScanner;

    private bool estAligne = false;

    private void Update()
    {
        // 1. Si on est déjà aligné, on ne fait plus rien
        if (estAligne) return;

        // 2. Vérifier si le casque VR a envoyé ses coordonnées sur le réseau
        VRCalibrationAnchor vrAnchor = FindAnyObjectByType<VRCalibrationAnchor>();
        if (vrAnchor == null || !vrAnchor.isCalibrated.Value)
        {
            return; // On attend que le joueur VR appuie sur son bouton
        }

        // 3. Vérifier si le téléphone AR a trouvé le marqueur physique
        if (arScanner == null || !arScanner.isMarkerFound)
        {
            return; // On attend que la caméra du téléphone croise l'image
        }

        // --- SI ON EST ICI, LES DEUX CONDITIONS SONT REMPLIES ---
        ExecuterAlignementAutomatique(vrAnchor);
    }

    private void ExecuterAlignementAutomatique(VRCalibrationAnchor vrAnchor)
    {
        Debug.Log("[COLOC] Conditions remplies ! Lancement du Snap...");

        // A. Ce que veut la VR (La cible absolue)
        Vector3 ciblePosition = vrAnchor.vrAnchorPosition.Value;
        float cibleRotationY = vrAnchor.vrAnchorRotation.Value.eulerAngles.y;

        // B. Ce que voit l'AR (La position actuelle du marqueur)
        Vector3 positionActuelle = arScanner.markerARPosition;
        float rotationActuelleY = arScanner.markerARRotation.eulerAngles.y;

        // C. ÉTAPE 1 : Aligner les rotations (On pivote le XR Origin autour du marqueur)
        float differenceAngle = cibleRotationY - rotationActuelleY;
        xrOrigin.transform.RotateAround(positionActuelle, Vector3.up, differenceAngle);

        // D. ÉTAPE 2 : Aligner les positions (On glisse le XR Origin pour superposer les points)
        Vector3 differencePosition = ciblePosition - positionActuelle;
        xrOrigin.transform.position += differencePosition;

        // E. On verrouille le système !
        estAligne = true;
        Debug.Log("[COLOC] SUCCÈS : Les mondes VR et AR sont parfaitement fusionnés et verrouillés !");
    }
}