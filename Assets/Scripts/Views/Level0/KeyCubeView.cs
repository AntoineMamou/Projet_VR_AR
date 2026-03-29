using UnityEngine;

/// <summary>
/// MVC — View
/// Responsable uniquement de l'affichage. Reçoit des ordres, ne décide rien.
/// À placer sur le même GameObject que KeyCubeController.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class KeyCubeView : MonoBehaviour
{
    // ── Sérialisation ──────────────────────────────────────────────────────
    [Header("Couleurs")]
    [SerializeField] private Color _defaultColor = Color.white;
    [SerializeField] private Color _inZoneColor  = Color.red;

    // ── Références ─────────────────────────────────────────────────────────
    private Renderer _renderer;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

    // ── Cycle Unity ────────────────────────────────────────────────────────
    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _renderer.material = new Material(_renderer.sharedMaterial);
        ApplyColor(false);
    }

    // ── API publique ───────────────────────────────────────────────────────
    /// <summary>Met à jour la couleur du cube selon son état.</summary>
    public void SetInZoneState(bool isInZone)
    {
        ApplyColor(isInZone);
    }

    /// <summary>Verrouille la position Z du cube pour contraindre le déplacement au plan XY.</summary>
    public void ApplyZConstraint(float lockedZ)
    {
        Vector3 pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, lockedZ);
    }

    // ── Interne ────────────────────────────────────────────────────────────
    private void ApplyColor(bool isInZone)
    {
        Color target = isInZone ? _inZoneColor : _defaultColor;

        if (_renderer.material.HasProperty(BaseColorID))
            _renderer.material.SetColor(BaseColorID, target);
        else
            _renderer.material.color = target;
    }
}