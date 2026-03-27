using UnityEngine;
using System.Collections.Generic;

public class UIPlacableManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private PlacableConfig _config;
    [SerializeField] private PlacableButton _buttonPrefab;
    [SerializeField] private Transform _placableBar;

    private List<PlacableButton> _spawnedButtons = new List<PlacableButton>();
    private string _selectedPlacableID = "";

    void Start()
    {
        InitializeBar();
    }

    private void InitializeBar()
    {
        foreach (Transform child in _placableBar) Destroy(child.gameObject);
        _spawnedButtons.Clear();

        foreach (var entry in _config.Objects)
        {
            PlacableButton btn = Instantiate(_buttonPrefab, _placableBar);
            btn.Setup(entry.id, entry.icon, OnPlacableSelected);
            _spawnedButtons.Add(btn);
        }
    }

    private void OnPlacableSelected(string id)
    {
        _selectedPlacableID = (_selectedPlacableID == id) ? "" : id;

        if (AppBootstrapper.TouchController != null)
        {
            AppBootstrapper.TouchController.SelectItem(_selectedPlacableID);
            UpdateVisualSelection();
        }
        else
        {
            Debug.LogWarning("[UIPlacableManager] TouchController non trouvé dans le Bootstrapper !");
        }
    }

    private void UpdateVisualSelection()
    {
        foreach (var btn in _spawnedButtons)
        {
            btn.SetVisualState(btn.Id == _selectedPlacableID);
        }
    }
}