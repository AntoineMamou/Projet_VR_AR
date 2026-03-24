using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class VictoryPanelController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject _panelRoot;

    [Header("XR Placement")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private bool _snapInFrontOfCameraOnShow = true;
    [SerializeField] private float _distanceFromCamera = 2f;
    [SerializeField] private Vector3 _panelOffset = new(0f, 0f, 0f);

    [Header("Stats")]
    [SerializeField] private TMP_Text _timeValueText;
    [SerializeField] private TMP_Text _cubesValueText;
    [SerializeField] private TMP_Text _buzzersValueText;
    [SerializeField] private string _timeTextFormat = "TEMPS : {0}";
    [SerializeField] private string _cubesTextFormat = "CUBES UTILISES : {0}";
    [SerializeField] private string _buzzersTextFormat = "BUZZERS TOUCHES : {0}";

    [Header("Stars")]
    [SerializeField] private Image[] _stars;
    [SerializeField] private Sprite _filledStarSprite;
    [SerializeField] private Sprite _emptyStarSprite;

    [Header("Scoring")]
    [SerializeField] private float _threeStarTimeLimit = 45f;
    [SerializeField] private float _twoStarTimeLimit = 90f;
    [SerializeField] private int _threeStarCubeLimit = 3;
    [SerializeField] private int _twoStarCubeLimit = 6;
    [SerializeField] private int _requiredBuzzersTouched = 3;
    [SerializeField] private bool _pauseGameWhenShown = true;

    private bool _isSubscribed;
    private CanvasGroup _panelCanvasGroup;

    private void Awake()
    {
        if (_panelRoot == null)
        {
            _panelRoot = gameObject;
        }

        EnsureCanvasGroupIfNeeded();
        CacheCameraTransform();
        SetPanelVisible(false);
    }

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Start()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (_isSubscribed && LevelRunStats.Instance != null)
        {
            LevelRunStats.Instance.LevelCompleted -= HandleLevelCompleted;
            _isSubscribed = false;
        }
    }

    private void HandleLevelCompleted(LevelResultData result)
    {
        Debug.Log("[VictoryPanelController] Received level completion event.");

        if (_timeValueText != null)
        {
            _timeValueText.text = string.Format(_timeTextFormat, FormatTime(result.ElapsedTime));
        }
        else
        {
            Debug.LogWarning("[VictoryPanelController] Time text is not assigned.");
        }

        if (_cubesValueText != null)
        {
            _cubesValueText.text = string.Format(_cubesTextFormat, result.CubesUsed);
        }
        else
        {
            Debug.LogWarning("[VictoryPanelController] Cubes text is not assigned.");
        }

        if (_buzzersValueText != null)
        {
            _buzzersValueText.text = string.Format(_buzzersTextFormat, result.BuzzersTouched);
        }
        else
        {
            Debug.LogWarning("[VictoryPanelController] Buzzers text is not assigned.");
        }

        UpdateStars(CalculateStars(result));

        if (_snapInFrontOfCameraOnShow)
        {
            MovePanelInFrontOfCamera();
        }

        SetPanelVisible(true);

        if (_pauseGameWhenShown)
        {
            Time.timeScale = 0f;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HidePanel()
    {
        Time.timeScale = 1f;
        SetPanelVisible(false);
    }

    [ContextMenu("Debug/Show Panel Now")]
    public void DebugShowPanelNow()
    {
        Debug.Log("[VictoryPanelController] Context menu show panel triggered.");
        HandleLevelCompleted(new LevelResultData(42f, 3, 2));
    }

    private int CalculateStars(LevelResultData result)
    {
        if (result.BuzzersTouched <= 0)
        {
            return 0;
        }

        int stars = 1;

        if (result.BuzzersTouched >= _requiredBuzzersTouched)
        {
            stars = 2;
        }

        if (result.BuzzersTouched >= _requiredBuzzersTouched &&
            result.ElapsedTime <= _threeStarTimeLimit &&
            result.CubesUsed <= _threeStarCubeLimit)
        {
            stars = 3;
        }
        else if (result.BuzzersTouched >= _requiredBuzzersTouched &&
                 result.ElapsedTime <= _twoStarTimeLimit &&
                 result.CubesUsed <= _twoStarCubeLimit)
        {
            stars = Mathf.Max(stars, 2);
        }

        return stars;
    }

    private void UpdateStars(int filledCount)
    {
        if (_stars == null)
        {
            Debug.LogWarning("[VictoryPanelController] Stars array is not assigned.");
            return;
        }

        for (int index = 0; index < _stars.Length; index++)
        {
            if (_stars[index] == null)
            {
                continue;
            }

            _stars[index].sprite = index < filledCount ? _filledStarSprite : _emptyStarSprite;
        }
    }

    private void SetPanelVisible(bool isVisible)
    {
        if (_panelRoot != null)
        {
            if (_panelRoot == gameObject)
            {
                EnsureCanvasGroupIfNeeded();

                if (_panelCanvasGroup != null)
                {
                    _panelCanvasGroup.alpha = isVisible ? 1f : 0f;
                    _panelCanvasGroup.interactable = isVisible;
                    _panelCanvasGroup.blocksRaycasts = isVisible;
                }

                return;
            }

            _panelRoot.SetActive(isVisible);
        }
    }

    private void TrySubscribe()
    {
        if (_isSubscribed || LevelRunStats.Instance == null)
        {
            if (LevelRunStats.Instance == null)
            {
                Debug.LogWarning("[VictoryPanelController] No LevelRunStats instance found. The panel will not react until one exists.");
            }

            return;
        }

        LevelRunStats.Instance.LevelCompleted += HandleLevelCompleted;
        _isSubscribed = true;
        Debug.Log("[VictoryPanelController] Subscribed to LevelRunStats.");
    }

    private void CacheCameraTransform()
    {
        if (_cameraTransform != null)
        {
            return;
        }

        if (Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    private void MovePanelInFrontOfCamera()
    {
        CacheCameraTransform();

        if (_cameraTransform == null || _panelRoot == null)
        {
            Debug.LogWarning("[VictoryPanelController] Cannot move panel in front of camera because camera or panel root is missing.");
            return;
        }

        Transform panelTransform = GetPlacementTransform();
        Vector3 targetPosition = _cameraTransform.position + (_cameraTransform.forward * _distanceFromCamera);
        targetPosition += _cameraTransform.TransformVector(_panelOffset);

        panelTransform.position = targetPosition;

        Vector3 forward = panelTransform.position - _cameraTransform.position;
        if (forward.sqrMagnitude > 0.0001f)
        {
            panelTransform.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
        }

        if (panelTransform != _panelRoot.transform)
        {
            _panelRoot.transform.localPosition = Vector3.zero;
            _panelRoot.transform.localRotation = Quaternion.identity;
        }
    }

    private void EnsureCanvasGroupIfNeeded()
    {
        if (_panelRoot != gameObject)
        {
            return;
        }

        _panelCanvasGroup = GetComponent<CanvasGroup>();
        if (_panelCanvasGroup == null)
        {
            _panelCanvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private Transform GetPlacementTransform()
    {
        Canvas parentCanvas = _panelRoot.GetComponentInParent<Canvas>();

        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.WorldSpace)
        {
            return parentCanvas.transform;
        }

        return _panelRoot.transform;
    }

    private static string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.CeilToInt(seconds);
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;
        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
