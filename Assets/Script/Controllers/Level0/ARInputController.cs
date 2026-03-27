using UnityEngine;

[RequireComponent(typeof(ARInputView))]
public class ARInputController : MonoBehaviour
{
    [Header("Cible")]
    [SerializeField] private KeyCubeController _keyCubeController;

    private ARInputModel _model;
    private ARInputView _view;

    private void Awake()
    {
        _model = new ARInputModel();
        _view = GetComponent<ARInputView>();

        _view.OnDirectionChanged += OnDirectionChanged;
        _model.OnControllingChanged += OnControllingChanged;
    }

    private void OnDestroy()
    {
        _view.OnDirectionChanged -= OnDirectionChanged;
        _model.OnControllingChanged -= OnControllingChanged;
    }

    private void Update()
    {
        if (LevelRunStats.AreInteractionsLocked)
        {
            if (_keyCubeController != null)
            {
                _keyCubeController.SetARControlling(false);
            }

            return;
        }

        if (!_model.IsControlling || _keyCubeController == null)
        {
            return;
        }

        _keyCubeController.MoveAlongZ(_model.Direction);
    }

    private void OnDirectionChanged(float direction)
    {
        if (LevelRunStats.AreInteractionsLocked)
        {
            return;
        }

        _model.Direction = direction;
    }

    private void OnControllingChanged(bool isControlling)
    {
        if (_keyCubeController == null)
        {
            return;
        }

        if (LevelRunStats.AreInteractionsLocked)
        {
            isControlling = false;
        }

        _keyCubeController.SetARControlling(isControlling);
    }
}
