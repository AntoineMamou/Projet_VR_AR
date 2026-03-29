using UnityEngine;

/// <summary>
/// MVC — View
/// Gère les retours visuels de la zone : feedback console et visibilité
/// de l'ExitDoor via RevealObjectView.
/// À placer sur le GameObject Zone avec ZoneController.
/// </summary>
public class ZoneView : MonoBehaviour
{
    // ── Sérialisation ──────────────────────────────────────────────────────
    [Header("Exit Door")]
    [SerializeField] private RevealObjectView _exitDoorRevealView;

    // ── API publique — KeyCube ─────────────────────────────────────────────
    public void OnCubeEntered(KeyCubeController cube)
    {
        Debug.Log($"[Zone] {cube.name} est entré dans la zone.", this);
    }

    public void OnCubeExited(KeyCubeController cube)
    {
        Debug.Log($"[Zone] {cube.name} a quitté la zone.", this);
    }

    // ── API publique — ExitDoor ────────────────────────────────────────────
    public void SetExitDoorVisible(bool visible)
    {
        if (_exitDoorRevealView == null)
        {
            Debug.LogWarning("[ZoneView] RevealObjectView de l'ExitDoor non assignée dans l'Inspector.", this);
            return;
        }

        if (visible)
            _exitDoorRevealView.ShowObject();
        else
            _exitDoorRevealView.HideObject();
    }
}