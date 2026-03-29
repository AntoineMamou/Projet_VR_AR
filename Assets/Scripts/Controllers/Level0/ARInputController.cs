using UnityEngine;


[RequireComponent(typeof(ARInputView))]
public class ARInputController : MonoBehaviour
{
    private KeyCubeController _keyCubeController;

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
        if (_keyCubeController == null)
        {
            TryFindKeyCube();

            if (_keyCubeController == null) return;
        }

        if (!_model.IsControlling) return;
        _keyCubeController.MoveAlongZ(_model.Direction);
    }

    private void TryFindKeyCube()
    {
        GameObject cubeObject = GameObject.FindGameObjectWithTag("KeyCube");

        if (cubeObject != null)
        {
            _keyCubeController = cubeObject.GetComponent<KeyCubeController>();
            Debug.Log("[ARInput] Succès : KeyCube trouvé et assigné automatiquement !");

            _keyCubeController.SetARControlling(_model.IsControlling);
        }
    }

    private void OnDirectionChanged(float direction)
    {
        _model.Direction = direction;
    }

    private void OnControllingChanged(bool isControlling)
    {
        if (_keyCubeController != null)
        {
            _keyCubeController.SetARControlling(isControlling);

            if (!isControlling)
            {
                _keyCubeController.MoveAlongZ(0f);
            }
        }
        else
        {
            Debug.LogWarning("[ARInput] Impossible d'envoyer l'ordre : KeyCube non trouvé !");
        }
    }
}