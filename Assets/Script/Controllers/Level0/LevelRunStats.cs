using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class LevelRunStats : NetworkBehaviour
{
    private const string LevelSceneName = "Level_Design_0";

    public static LevelRunStats Instance { get; private set; }

    public static event Action<LevelRunStats> InstanceChanged;

    public event Action<LevelResultData> LevelCompleted;

    private readonly NetworkVariable<float> _startServerTime = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<float> _completedElapsedTime = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<bool> _redKeyActivated = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<bool> _greenKeyActivated = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<bool> _exitUnlocked = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private readonly NetworkVariable<bool> _isCompleted = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private bool _hasRaisedLevelCompleted;
    private bool _restartRequested;

    public float ElapsedTime
    {
        get
        {
            if (!IsSpawned || NetworkManager == null)
            {
                return 0f;
            }

            if (_isCompleted.Value)
            {
                return _completedElapsedTime.Value;
            }

            return Mathf.Max(0f, (float)NetworkManager.ServerTime.Time - _startServerTime.Value);
        }
    }

    public bool RedKeyActivated => _redKeyActivated.Value;
    public bool GreenKeyActivated => _greenKeyActivated.Value;
    public bool ExitUnlocked => _exitUnlocked.Value;
    public bool IsCompleted => _isCompleted.Value;
    public int KeysActivated => (RedKeyActivated ? 1 : 0) + (GreenKeyActivated ? 1 : 0);
    public LevelResultData CurrentResult => new LevelResultData(ElapsedTime, RedKeyActivated, GreenKeyActivated, ExitUnlocked);

    public static bool AreInteractionsLocked => Instance != null && Instance.IsCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InstanceChanged?.Invoke(this);
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        Instance = null;
        InstanceChanged?.Invoke(null);
    }

    public override void OnNetworkSpawn()
    {
        _isCompleted.OnValueChanged += HandleCompletionChanged;

        if (IsServer)
        {
            ResetRunState();
        }

        if (_isCompleted.Value)
        {
            RaiseLevelCompletedIfNeeded();
        }
    }

    public override void OnNetworkDespawn()
    {
        _isCompleted.OnValueChanged -= HandleCompletionChanged;
        _hasRaisedLevelCompleted = false;
        _restartRequested = false;
    }

    public void RegisterRedKeyActivated()
    {
        if (!CanProcessStatMutation())
        {
            return;
        }

        if (IsServer)
        {
            SetRedKeyActivated();
            return;
        }

        RegisterRedKeyActivatedServerRpc();
    }

    public void RegisterExitUnlocked()
    {
        if (!CanProcessStatMutation())
        {
            return;
        }

        if (IsServer)
        {
            SetExitUnlocked();
            return;
        }

        RegisterExitUnlockedServerRpc();
    }

    public void RegisterGreenKeyActivatedAndComplete()
    {
        if (!CanProcessStatMutation())
        {
            return;
        }

        if (IsServer)
        {
            CompleteLevelInternal();
            return;
        }

        RegisterGreenKeyActivatedAndCompleteServerRpc();
    }

    public void RequestRestart()
    {
        if (!IsSpawned || NetworkManager == null)
        {
            return;
        }

        if (IsServer)
        {
            ReloadLevelForAll();
            return;
        }

        RequestRestartServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RegisterRedKeyActivatedServerRpc()
    {
        SetRedKeyActivated();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RegisterExitUnlockedServerRpc()
    {
        SetExitUnlocked();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RegisterGreenKeyActivatedAndCompleteServerRpc()
    {
        CompleteLevelInternal();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestRestartServerRpc()
    {
        ReloadLevelForAll();
    }

    private bool CanProcessStatMutation()
    {
        return IsSpawned && NetworkManager != null && !_isCompleted.Value;
    }

    private void ResetRunState()
    {
        _startServerTime.Value = (float)NetworkManager.ServerTime.Time;
        _completedElapsedTime.Value = 0f;
        _redKeyActivated.Value = false;
        _greenKeyActivated.Value = false;
        _exitUnlocked.Value = false;
        _isCompleted.Value = false;
        _restartRequested = false;
    }

    private void SetRedKeyActivated()
    {
        if (_isCompleted.Value || _redKeyActivated.Value)
        {
            return;
        }

        _redKeyActivated.Value = true;
    }

    private void SetExitUnlocked()
    {
        if (_isCompleted.Value || _exitUnlocked.Value)
        {
            return;
        }

        _exitUnlocked.Value = true;
    }

    private void CompleteLevelInternal()
    {
        if (_isCompleted.Value)
        {
            return;
        }

        _greenKeyActivated.Value = true;
        _completedElapsedTime.Value = Mathf.Max(0f, (float)NetworkManager.ServerTime.Time - _startServerTime.Value);
        _isCompleted.Value = true;
    }

    private void ReloadLevelForAll()
    {
        if (_restartRequested || NetworkManager == null || NetworkManager.SceneManager == null)
        {
            return;
        }

        _restartRequested = true;
        NetworkManager.SceneManager.LoadScene(LevelSceneName, LoadSceneMode.Single);
    }

    private void HandleCompletionChanged(bool previousValue, bool newValue)
    {
        if (!previousValue && newValue)
        {
            RaiseLevelCompletedIfNeeded();
            return;
        }

        if (!newValue)
        {
            _hasRaisedLevelCompleted = false;
        }
    }

    private void RaiseLevelCompletedIfNeeded()
    {
        if (_hasRaisedLevelCompleted)
        {
            return;
        }

        _hasRaisedLevelCompleted = true;
        LevelCompleted?.Invoke(CurrentResult);
    }
}

[Serializable]
public readonly struct LevelResultData
{
    public readonly float ElapsedTime;
    public readonly int KeysActivated;
    public readonly bool ExitUnlocked;
    public readonly bool RedKeyActivated;
    public readonly bool GreenKeyActivated;

    public LevelResultData(float elapsedTime, bool redKeyActivated, bool greenKeyActivated, bool exitUnlocked)
    {
        ElapsedTime = elapsedTime;
        RedKeyActivated = redKeyActivated;
        GreenKeyActivated = greenKeyActivated;
        ExitUnlocked = exitUnlocked;
        KeysActivated = (redKeyActivated ? 1 : 0) + (greenKeyActivated ? 1 : 0);
    }
}
