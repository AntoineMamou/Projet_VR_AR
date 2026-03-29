using System;
using System.Collections.Generic;

/// <summary>
/// MVC — Model
/// Garde la liste des KeyCubes dans la zone et pilote l'ExitDoorModel
/// en conséquence.
/// </summary>
public class ZoneModel
{
    // ── État ───────────────────────────────────────────────────────────────
    private readonly HashSet<KeyCubeController> _cubesInZone = new();
    private readonly ExitDoorModel _exitDoorModel;

    public IReadOnlyCollection<KeyCubeController> CubesInZone => _cubesInZone;

    // ── Événements ─────────────────────────────────────────────────────────
    /// <summary>Déclenché quand un cube entre ou sort. bool = true si entrée.</summary>
    public event Action<KeyCubeController, bool> OnZoneChanged;

    // ── Construction ───────────────────────────────────────────────────────
    public ZoneModel(ExitDoorModel exitDoorModel)
    {
        _exitDoorModel = exitDoorModel;
    }

    // ── Mutation ───────────────────────────────────────────────────────────
    public bool AddCube(KeyCubeController cube)
    {
        if (!_cubesInZone.Add(cube)) return false;
        OnZoneChanged?.Invoke(cube, true);
        RefreshExitDoor();
        return true;
    }

    public bool RemoveCube(KeyCubeController cube)
    {
        if (!_cubesInZone.Remove(cube)) return false;
        OnZoneChanged?.Invoke(cube, false);
        RefreshExitDoor();
        return true;
    }

    // ── Logique ────────────────────────────────────────────────────────────
    /// <summary>
    /// L'ExitDoor est visible dès qu'au moins un KeyCube est dans la zone.
    /// </summary>
    private void RefreshExitDoor()
    {
        _exitDoorModel.IsVisible = _cubesInZone.Count > 0;
    }
}