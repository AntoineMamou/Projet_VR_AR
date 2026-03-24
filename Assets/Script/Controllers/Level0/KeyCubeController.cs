using UnityEngine;

/// <summary>
/// MVC — Controller
/// Fait le lien entre le Model et la View du KeyCube.
/// Exposé publiquement pour que la ZoneController puisse l'appeler.
/// À placer sur le GameObject KeyCube avec KeyCubeView.
/// </summary>
[RequireComponent(typeof(KeyCubeView))]
public class KeyCubeController : MonoBehaviour
{
    // ── Références ─────────────────────────────────────────────────────────
    private KeyCubeModel _model;
    private KeyCubeView  _view;

    // ── Cycle Unity ────────────────────────────────────────────────────────
    private void Awake()
    {
        _model = new KeyCubeModel();
        _view  = GetComponent<KeyCubeView>();

        // Le Controller s'abonne aux changements du Model pour mettre à jour la View.
        _model.OnStateChanged += OnModelStateChanged;
    }

    private void OnDestroy()
    {
        _model.OnStateChanged -= OnModelStateChanged;
    }

    // ── API publique ── appelée par ZoneController ─────────────────────────
    public void EnterZone()
    {
        _model.IsInZone = true;   // déclenche OnStateChanged → View se met à jour
    }

    public void ExitZone()
    {
        _model.IsInZone = false;
    }

    // ── Réaction au Model ──────────────────────────────────────────────────
    private void OnModelStateChanged(bool isInZone)
    {
        _view.SetInZoneState(isInZone);
    }
}