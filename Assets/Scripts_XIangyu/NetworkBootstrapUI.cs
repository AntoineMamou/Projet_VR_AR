using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class NetworkBootstrapUI : MonoBehaviour
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    [SerializeField] private GameObject _menuRoot;
    [SerializeField] private GameSessionManager _gameSessionManager;

    private void Awake()
    {
        if (_hostButton != null)
        {
            _hostButton.onClick.AddListener(StartAsHost);
        }

        if (_clientButton != null)
        {
            _clientButton.onClick.AddListener(StartAsClient);
        }
    }

    private void OnDestroy()
    {
        if (_hostButton != null)
        {
            _hostButton.onClick.RemoveListener(StartAsHost);
        }

        if (_clientButton != null)
        {
            _clientButton.onClick.RemoveListener(StartAsClient);
        }
    }

    public void StartAsHost()
    {
        StartNetworkSession(true);
    }

    public void StartAsClient()
    {
        StartNetworkSession(false);
    }

    private void StartNetworkSession(bool startAsHost)
    {
        var networkManager = NetworkManager.Singleton;

        if (networkManager == null)
        {
            Debug.LogError("[NetworkBootstrapUI] No NetworkManager singleton found in the scene.");
            return;
        }

        if (_gameSessionManager != null)
        {
            _gameSessionManager.ConfigureNetworkManager();
        }

        if (networkManager.IsListening)
        {
            Debug.LogWarning("[NetworkBootstrapUI] A network session is already running.");
            return;
        }

        SetButtonsInteractable(false);

        bool didStart = startAsHost
            ? networkManager.StartHost()
            : networkManager.StartClient();

        if (!didStart)
        {
            Debug.LogError("[NetworkBootstrapUI] Failed to start the requested network session.");
            SetButtonsInteractable(true);
            return;
        }

        if (_menuRoot != null)
        {
            _menuRoot.SetActive(false);
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (_hostButton != null)
        {
            _hostButton.interactable = interactable;
        }

        if (_clientButton != null)
        {
            _clientButton.interactable = interactable;
        }
    }
}
