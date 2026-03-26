using Unity.Netcode;
using UnityEngine;
using Unity.XR.CoreUtils;

public class ARColocationManager : MonoBehaviour
{
    [Header("Références")]
    public XROrigin xrOrigin;
    public ARMarkerScanner arScanner;

    [Header("Paramètres de stabilisation")]
    public float tempsStabilisation = 2.5f;
    private float chrono = 0f;
    private bool estAligne = false;

    // L'objet fantôme qui servira de "referencePose" (La vérité terrain de la VR)
    private Transform referencePoseFantomeVR;

    private void Update()
    {
        // --- CORRECTIF ANTI-RANDOM ---
        // Si on est déjà aligné, on ne rentre plus JAMAIS dans la logique
        if (estAligne) return;

        // 1. VR Prête ?
        VRCalibrationAnchor vrAnchor = FindAnyObjectByType<VRCalibrationAnchor>();
        if (vrAnchor == null || !vrAnchor.isCalibrated.Value) return;

        // 2. AR Prête et STABLE ?
        if (arScanner == null || !arScanner.isMarkerFound || arScanner.activeMarkerTransform == null)
        {
            chrono = 0f; // Reset timer
            return;
        }

        // On attend la stabilisation de la profondeur AR
        chrono += Time.deltaTime;
        if (chrono >= tempsStabilisation)
        {
            ExecuterCalibrationOfficielle(vrAnchor);
        }
    }

    private void ExecuterCalibrationOfficielle(VRCalibrationAnchor vrAnchor)
    {
        Debug.Log("[COLOC] Stabilisation terminée, appel du script de Calibration !");

        // 1. CRÉATION DU FANTÔME VR (Le referencePose)
        if (referencePoseFantomeVR == null)
        {
            referencePoseFantomeVR = new GameObject("VRPose_GroundTruth").transform;
        }
        // On lui donne les coordonnées reçues par le réseau
        referencePoseFantomeVR.position = vrAnchor.vrAnchorPosition.Value;
        referencePoseFantomeVR.rotation = vrAnchor.vrAnchorRotation.Value;


        // 2. PRÉPARATION DES VARIABLES POUR LE SCRIPT FOURNI
        // ObjectToCalibrate : L'objet global qu'on veut déplacer (Le monde AR)
        Transform objectToCalibrate = xrOrigin.transform;

        // PoseToAlign : L'élément dans le monde AR qui doit s'aligner (Le marqueur)
        Transform poseToAlign = arScanner.activeMarkerTransform;

        // ReferencePose : L'endroit où il doit aller (Le fantôme VR)
        Transform referencePose = referencePoseFantomeVR;


        // 3. LE COUP DE MAGIE OFFICIEL

        Calibration.Calibrate(objectToCalibrate, poseToAlign, referencePose);

        estAligne = true;
        Debug.Log("[COLOC] SUCCÈS : Les mondes sont parfaitement alignés avec la nouvelle méthode !");
    }
}