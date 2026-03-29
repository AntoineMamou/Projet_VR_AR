using UnityEngine;

/// <summary>
/// MVC — Controller
/// Point d'entrée des triggers Unity pour la zone.
/// Orchestre ZoneModel, ExitDoorModel et ZoneView.
/// À placer sur le GameObject Zone avec ZoneView et un BoxCollider (Is Trigger).
/// </summary>
[RequireComponent(typeof(ZoneView))]
public class ZoneController : MonoBehaviour
{
    // ── Constantes ─────────────────────────────────────────────────────────
    private const string KeyCubeTag = "KeyCube";

    // ── Références ─────────────────────────────────────────────────────────
    private ZoneModel     _zoneModel;
    private ExitDoorModel _exitDoorModel;
    private ZoneView      _view;

    // ── Cycle Unity ────────────────────────────────────────────────────────
    private void Awake()
    {
        _view          = GetComponent<ZoneView>();
        _exitDoorModel = new ExitDoorModel();
        _zoneModel     = new ZoneModel(_exitDoorModel);

        _zoneModel.OnZoneChanged            += OnZoneChanged;
        _exitDoorModel.OnVisibilityChanged  += OnExitDoorVisibilityChanged;
    }

    private void OnDestroy()
    {
        _zoneModel.OnZoneChanged            -= OnZoneChanged;
        _exitDoorModel.OnVisibilityChanged  -= OnExitDoorVisibilityChanged;
    }

    // ── Détection (Trigger) ────────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(KeyCubeTag)) return;
        if (other.TryGetComponent(out KeyCubeController cube))
            _zoneModel.AddCube(cube);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(KeyCubeTag)) return;
        if (other.TryGetComponent(out KeyCubeController cube))
            _zoneModel.RemoveCube(cube);
    }

    // ── Réaction au ZoneModel ──────────────────────────────────────────────
    private void OnZoneChanged(KeyCubeController cube, bool entered)
    {
        if (entered)
        {
            cube.EnterZone();
            _view.OnCubeEntered(cube);
        }
        else
        {
            cube.ExitZone();
            _view.OnCubeExited(cube);
        }
    }

    // ── Réaction à l'ExitDoorModel ─────────────────────────────────────────
    private void OnExitDoorVisibilityChanged(bool visible)
    {
        _view.SetExitDoorVisible(visible);
    }
}