using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRCalibrationAnchor : NetworkBehaviour
{

    public Transform rightControllerTransform;

   //XRI Right Interaction/Select
    public InputActionReference calibrateButton;

    // --- VARIABLES RÉSEAU (Synchronisées automatiquement vers l'AR) ---
    // ReadPermission.Everyone = L'AR peut les lire.
    // WritePermission.Server = Seul le casque VR peut les modifier.

    public NetworkVariable<Vector3> vrAnchorPosition = new NetworkVariable<Vector3>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<Quaternion> vrAnchorRotation = new NetworkVariable<Quaternion>(
        default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> isCalibrated = new NetworkVariable<bool>(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private GameObject repereVisuelVR;

    // Activer l'écoute du bouton
    private void OnEnable()
    {
        if (calibrateButton != null)
        {
            calibrateButton.action.Enable();
            calibrateButton.action.performed += OnCalibratePressed;
        }
    }

    // Désactiver l'écoute
    private void OnDisable()
    {
        if (calibrateButton != null)
        {
            calibrateButton.action.performed -= OnCalibratePressed;
            calibrateButton.action.Disable();
        }
    }

    // Fonction appelée quand le joueur appuie sur le bouton
    private void OnCalibratePressed(InputAction.CallbackContext context)
    {
        //Seul le Serveur (le casque VR) a le droit de définir l'ancrage
        //if (!IsServer || !IsHost) return;
        Debug.LogWarning("PRESSED");

        if (rightControllerTransform != null && !isCalibrated.Value)
        {
            // On sauvegarde la position et la rotation de la manette dans les variables réseau
            Vector3 directionAvant = rightControllerTransform.forward;
            directionAvant.y = 0;
            Quaternion rotationPlate = Quaternion.LookRotation(directionAvant, Vector3.up);

            vrAnchorPosition.Value = rightControllerTransform.position;
            vrAnchorRotation.Value = rotationPlate;
            isCalibrated.Value = true;

            Debug.Log($"[VERIFICATION HAUTEUR] Le marqueur VR a été posé à une hauteur Y de : {vrAnchorPosition.Value.y} mètres par rapport au sol virtuel.");


            if (repereVisuelVR == null)
            {
                // On crée un petit cube
                repereVisuelVR = GameObject.CreatePrimitive(PrimitiveType.Cube);

                // On l'aplatit pour qu'il ressemble à un marqueur au sol (10cm x 1cm x 10cm)
                repereVisuelVR.transform.localScale = new Vector3(0.1f, 0.01f, 0.1f);

                // On le peint en vert fluo
                repereVisuelVR.GetComponent<Renderer>().material.color = Color.green;
            }

            // On place le repère vert exactement là où la manette a cliqué
            repereVisuelVR.transform.position = vrAnchorPosition.Value;
            repereVisuelVR.transform.rotation = vrAnchorRotation.Value;

            Debug.Log($"[CALIBRATION VR] Ancrage défini à la position : {vrAnchorPosition.Value}");
        }
        else
        {
            Debug.LogError("[CALIBRATION VR] Erreur : La manette n'est pas assignée dans l'inspecteur !");
        }
    }
}