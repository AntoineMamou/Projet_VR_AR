using UnityEngine;
using UnityEngine.UI;

public class PlacableButton : MonoBehaviour
{
    [Header("Branchements")]
    [SerializeField] private Image _iconDisplay;
    [SerializeField] private GameObject _borderVisual;
    [SerializeField] private Button _clickDetector;

    private string _placableId;
    private System.Action<string> _onNotifyManager;

    public string Id => _placableId;

    public void Setup(string id, Sprite icon, System.Action<string> onClickAction)
    {
        _placableId = id;
        _onNotifyManager = onClickAction;
        _iconDisplay.sprite = icon;

        _clickDetector.onClick.RemoveAllListeners();
        _clickDetector.onClick.AddListener(() => _onNotifyManager?.Invoke(_placableId));
    }

    public void SetVisualState(bool isSelected)
    {
        _borderVisual.SetActive(isSelected);
    }
}