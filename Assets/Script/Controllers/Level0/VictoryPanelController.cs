using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

[DisallowMultipleComponent]
public class VictoryPanelController : MonoBehaviour
{
    private const string LevelSceneName = "Level_Design_0";

    [Header("Placement")]
    [SerializeField] private float _distanceFromCamera = 1.6f;
    [SerializeField] private Vector3 _panelOffset = new Vector3(0f, -0.08f, 0f);

    [Header("Scoring")]
    [SerializeField] private float _threeStarTimeLimit = 90f;

    private Camera _playerCamera;
    private LevelRunStats _subscribedStats;
    private CanvasGroup _canvasGroup;
    private GameObject _canvasRoot;
    private GameObject _panelRoot;
    private TextMeshProUGUI _timeText;
    private TextMeshProUGUI _keysText;
    private TextMeshProUGUI _exitText;
    private TextMeshProUGUI[] _stars;
    private Button _restartButton;

    private void Awake()
    {
        LevelRunStats.InstanceChanged += HandleStatsInstanceChanged;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnEnable()
    {
        TrySubscribe(LevelRunStats.Instance);
    }

    private void OnDisable()
    {
        Unsubscribe();
        HidePanel();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        LevelRunStats.InstanceChanged -= HandleStatsInstanceChanged;
        Unsubscribe();
    }

    public void Initialize(Camera playerCamera)
    {
        if (_playerCamera == playerCamera && _canvasRoot != null)
        {
            return;
        }

        _playerCamera = playerCamera;
        EnsurePanelBuilt();
        TrySubscribe(LevelRunStats.Instance);
    }

    private void HandleStatsInstanceChanged(LevelRunStats stats)
    {
        TrySubscribe(stats);
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        HidePanel();
        Unsubscribe();

        if (scene.name != LevelSceneName)
        {
            return;
        }

        TrySubscribe(LevelRunStats.Instance);
    }

    private void TrySubscribe(LevelRunStats stats)
    {
        if (stats == _subscribedStats)
        {
            if (stats != null && stats.IsCompleted)
            {
                HandleLevelCompleted(stats.CurrentResult);
            }

            return;
        }

        Unsubscribe();

        if (stats == null)
        {
            return;
        }

        _subscribedStats = stats;
        _subscribedStats.LevelCompleted += HandleLevelCompleted;

        if (_subscribedStats.IsCompleted)
        {
            HandleLevelCompleted(_subscribedStats.CurrentResult);
        }
    }

    private void Unsubscribe()
    {
        if (_subscribedStats == null)
        {
            return;
        }

        _subscribedStats.LevelCompleted -= HandleLevelCompleted;
        _subscribedStats = null;
    }

    private void HandleLevelCompleted(LevelResultData result)
    {
        EnsurePanelBuilt();

        if (_panelRoot == null)
        {
            return;
        }

        _timeText.text = $"TEMPS : {FormatTime(result.ElapsedTime)}";
        _keysText.text = $"CLES ACTIVEES : {result.KeysActivated}/2";
        _exitText.text = $"SORTIE DEVERROUILLEE : {(result.ExitUnlocked ? "OUI" : "NON")}";

        UpdateStars(CalculateStars(result));
        MovePanelInFrontOfCamera();
        SetPanelVisible(true);
    }

    private int CalculateStars(LevelResultData result)
    {
        if (!result.GreenKeyActivated)
        {
            return 0;
        }

        if (!result.RedKeyActivated)
        {
            return 1;
        }

        if (result.ExitUnlocked && result.ElapsedTime <= _threeStarTimeLimit)
        {
            return 3;
        }

        return 2;
    }

    private void UpdateStars(int filledCount)
    {
        if (_stars == null)
        {
            return;
        }

        for (int index = 0; index < _stars.Length; index++)
        {
            TextMeshProUGUI star = _stars[index];
            if (star == null)
            {
                continue;
            }

            bool isFilled = index < filledCount;
            star.text = isFilled ? "\u2605" : "\u2606";
            star.color = isFilled
                ? new Color32(255, 212, 92, 255)
                : new Color32(107, 122, 140, 255);
        }
    }

    private void EnsurePanelBuilt()
    {
        if (_canvasRoot != null || _playerCamera == null)
        {
            return;
        }

        TMP_FontAsset fontAsset = ResolveFontAsset();
        Sprite panelSprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");

        _canvasRoot = new GameObject(
            "VictoryCanvas",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster),
            typeof(TrackedDeviceGraphicRaycaster),
            typeof(CanvasGroup));

        _canvasRoot.transform.SetParent(_playerCamera.transform, false);

        Canvas canvas = _canvasRoot.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = _playerCamera;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = _canvasRoot.GetComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        RectTransform canvasRect = (RectTransform)_canvasRoot.transform;
        canvasRect.sizeDelta = new Vector2(900f, 640f);
        canvasRect.localScale = Vector3.one * 0.002f;

        _canvasGroup = _canvasRoot.GetComponent<CanvasGroup>();

        _panelRoot = CreateImage("VictoryPanel", _canvasRoot.transform, panelSprite, new Color32(18, 33, 49, 235));
        RectTransform panelRect = (RectTransform)_panelRoot.transform;
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(760f, 500f);
        panelRect.anchoredPosition = Vector2.zero;

        CreateText(
            "Title",
            _panelRoot.transform,
            fontAsset,
            "VICTOIRE",
            54f,
            FontStyles.Bold,
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(0f, -54f),
            new Vector2(620f, 70f),
            TextAlignmentOptions.Center);

        _stars = new TextMeshProUGUI[3];
        for (int index = 0; index < _stars.Length; index++)
        {
            _stars[index] = CreateText(
                $"Star{index + 1}",
                _panelRoot.transform,
                fontAsset,
                "\u2606",
                56f,
                FontStyles.Normal,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(-84f + (index * 84f), -130f),
                new Vector2(72f, 72f),
                TextAlignmentOptions.Center);
        }

        _timeText = CreateStatLine("TimeText", fontAsset, new Vector2(0f, 66f));
        _keysText = CreateStatLine("KeysText", fontAsset, new Vector2(0f, -4f));
        _exitText = CreateStatLine("ExitText", fontAsset, new Vector2(0f, -74f));

        _restartButton = CreateButton(_panelRoot.transform, fontAsset, panelSprite);
        _restartButton.onClick.AddListener(HandleRestartClicked);

        SetPanelVisible(false);
    }

    private TextMeshProUGUI CreateStatLine(string objectName, TMP_FontAsset fontAsset, Vector2 anchoredPosition)
    {
        return CreateText(
            objectName,
            _panelRoot.transform,
            fontAsset,
            string.Empty,
            34f,
            FontStyles.Normal,
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            anchoredPosition,
            new Vector2(620f, 48f),
            TextAlignmentOptions.Center);
    }

    private Button CreateButton(Transform parent, TMP_FontAsset fontAsset, Sprite panelSprite)
    {
        GameObject buttonObject = CreateImage("RestartButton", parent, panelSprite, new Color32(70, 128, 111, 255));
        RectTransform buttonRect = (RectTransform)buttonObject.transform;
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0f);
        buttonRect.anchoredPosition = new Vector2(0f, 34f);
        buttonRect.sizeDelta = new Vector2(260f, 72f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();

        TextMeshProUGUI label = CreateText(
            "Label",
            buttonObject.transform,
            fontAsset,
            "RESTART",
            30f,
            FontStyles.Bold,
            Vector2.zero,
            Vector2.one,
            new Vector2(0.5f, 0.5f),
            Vector2.zero,
            Vector2.zero,
            TextAlignmentOptions.Center);

        label.rectTransform.offsetMin = Vector2.zero;
        label.rectTransform.offsetMax = Vector2.zero;
        return button;
    }

    private static GameObject CreateImage(string objectName, Transform parent, Sprite sprite, Color color)
    {
        GameObject imageObject = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        Image image = imageObject.GetComponent<Image>();
        image.sprite = sprite;
        image.type = Image.Type.Sliced;
        image.color = color;
        return imageObject;
    }

    private static TextMeshProUGUI CreateText(
        string objectName,
        Transform parent,
        TMP_FontAsset fontAsset,
        string content,
        float fontSize,
        FontStyles fontStyle,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 pivot,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
        if (fontAsset != null)
        {
            text.font = fontAsset;
        }

        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = alignment;
        text.color = new Color32(245, 247, 250, 255);
        text.raycastTarget = false;

        RectTransform rect = text.rectTransform;
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        return text;
    }

    private TMP_FontAsset ResolveFontAsset()
    {
        if (TMP_Settings.defaultFontAsset != null)
        {
            return TMP_Settings.defaultFontAsset;
        }

        return Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
    }

    private void HandleRestartClicked()
    {
        LevelRunStats.Instance?.RequestRestart();
    }

    private void MovePanelInFrontOfCamera()
    {
        if (_canvasRoot == null)
        {
            return;
        }

        RectTransform canvasRect = (RectTransform)_canvasRoot.transform;
        canvasRect.localPosition = Vector3.forward * _distanceFromCamera + _panelOffset;
        canvasRect.localRotation = Quaternion.identity;
    }

    private void HidePanel()
    {
        SetPanelVisible(false);
    }

    private void SetPanelVisible(bool visible)
    {
        if (_canvasGroup == null)
        {
            return;
        }

        _canvasGroup.alpha = visible ? 1f : 0f;
        _canvasGroup.interactable = visible;
        _canvasGroup.blocksRaycasts = visible;
    }

    private static string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.CeilToInt(seconds);
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;
        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
