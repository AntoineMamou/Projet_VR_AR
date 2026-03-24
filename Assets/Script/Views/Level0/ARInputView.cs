using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// MVC — View (AR)
/// Utilise EventTrigger sur chaque bouton pour détecter PointerDown/PointerUp,
/// plus fiable que IPointerDownHandler sur Android.
/// À placer sur un GameObject UI Canvas (Screen Space - Overlay).
/// </summary>
public class ARInputView : MonoBehaviour
{
    // ── Sérialisation ──────────────────────────────────────────────────────
    [Header("Boutons")]
    [SerializeField] private GameObject _buttonLeft;
    [SerializeField] private GameObject _buttonRight;

    // ── Événements ─────────────────────────────────────────────────────────
    public event System.Action<float> OnDirectionChanged;

    // ── État interne ───────────────────────────────────────────────────────
    private bool _leftHeld;
    private bool _rightHeld;

    // ── Cycle Unity ────────────────────────────────────────────────────────
    private void Awake()
    {
        RegisterButton(_buttonLeft,
            onDown: () => { _leftHeld = true;  BroadcastDirection(); },
            onUp:   () => { _leftHeld = false; BroadcastDirection(); });

        RegisterButton(_buttonRight,
            onDown: () => { _rightHeld = true;  BroadcastDirection(); },
            onUp:   () => { _rightHeld = false; BroadcastDirection(); });
    }

    // ── Interne ────────────────────────────────────────────────────────────
    private void RegisterButton(GameObject go, System.Action onDown, System.Action onUp)
    {
        if (go == null)
        {
            Debug.LogWarning("[ARInputView] Bouton non assigné dans l'Inspector.", this);
            return;
        }

        EventTrigger trigger = go.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = go.AddComponent<EventTrigger>();

        // PointerDown
        var down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener(_ => onDown());
        trigger.triggers.Add(down);

        // PointerUp
        var up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener(_ => onUp());
        trigger.triggers.Add(up);
    }

    private void BroadcastDirection()
    {
        float dir = 0f;
        if (_rightHeld) dir += 1f;
        if (_leftHeld)  dir -= 1f;
        Debug.Log($"[ARInput] BroadcastDirection — dir={dir}, leftHeld={_leftHeld}, rightHeld={_rightHeld}");
        OnDirectionChanged?.Invoke(dir);
    }

    private void OnDisable()
    {
        _leftHeld  = false;
        _rightHeld = false;
        OnDirectionChanged?.Invoke(0f);
    }
}