using UnityEngine;

[RequireComponent(typeof(ZoneView))]
public class ZoneController : MonoBehaviour
{
    private const string KeyCubeTag = "KeyCube";

    private ZoneModel _zoneModel;
    private ExitDoorModel _exitDoorModel;
    private ZoneView _view;

    private void Awake()
    {
        _view = GetComponent<ZoneView>();
        _exitDoorModel = new ExitDoorModel();
        _zoneModel = new ZoneModel(_exitDoorModel);

        _zoneModel.OnZoneChanged += OnZoneChanged;
        _exitDoorModel.OnVisibilityChanged += OnExitDoorVisibilityChanged;
    }

    private void OnDestroy()
    {
        _zoneModel.OnZoneChanged -= OnZoneChanged;
        _exitDoorModel.OnVisibilityChanged -= OnExitDoorVisibilityChanged;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LevelRunStats.AreInteractionsLocked)
        {
            return;
        }

        if (!other.CompareTag(KeyCubeTag))
        {
            return;
        }

        if (other.TryGetComponent(out KeyCubeController cube))
        {
            _zoneModel.AddCube(cube);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (LevelRunStats.AreInteractionsLocked)
        {
            return;
        }

        if (!other.CompareTag(KeyCubeTag))
        {
            return;
        }

        if (other.TryGetComponent(out KeyCubeController cube))
        {
            _zoneModel.RemoveCube(cube);
        }
    }

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

    private void OnExitDoorVisibilityChanged(bool visible)
    {
        _view.SetExitDoorVisible(visible);

        if (visible)
        {
            LevelRunStats.Instance?.RegisterExitUnlocked();
        }
    }
}
