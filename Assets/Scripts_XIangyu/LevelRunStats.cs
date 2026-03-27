using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class LevelRunStats : MonoBehaviour
{
    public static LevelRunStats Instance { get; private set; }

    public event Action<LevelResultData> LevelCompleted;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugCompleteShortcut = false;
    [SerializeField] private KeyCode _debugCompleteKey = KeyCode.P;

    public float ElapsedTime => _isCompleted ? _completedElapsedTime : Time.time - _startTime;
    public int CubesUsed => _cubesUsed;
    public int BuzzersTouched => _buzzersTouched;
    public bool IsCompleted => _isCompleted;

    private float _startTime;
    private float _completedElapsedTime;
    private int _cubesUsed;
    private int _buzzersTouched;
    private bool _isCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResetRun();
    }

    private void Update()
    {
        if (_enableDebugCompleteShortcut && IsDebugCompletePressed())
        {
            Debug.Log("[LevelRunStats] Debug complete shortcut triggered.");
            CompleteLevel();
        }
    }

    [ContextMenu("Debug/Complete Level")]
    public void DebugCompleteLevel()
    {
        Debug.Log("[LevelRunStats] Context menu complete triggered.");
        CompleteLevel();
    }

    public void ResetRun()
    {
        _startTime = Time.time;
        _completedElapsedTime = 0f;
        _cubesUsed = 0;
        _buzzersTouched = 0;
        _isCompleted = false;
    }

    public void RegisterCubeUsed()
    {
        if (_isCompleted)
        {
            return;
        }

        _cubesUsed++;
        Debug.Log($"[LevelRunStats] Cube used. Total cubes: {_cubesUsed}.");
    }

    public void RegisterBuzzerTouched()
    {
        if (_isCompleted)
        {
            return;
        }

        _buzzersTouched++;
        Debug.Log($"[LevelRunStats] Buzzer touched. Total buzzers: {_buzzersTouched}.");
    }

    public void CompleteLevel()
    {
        if (_isCompleted)
        {
            return;
        }

        _isCompleted = true;
        _completedElapsedTime = Time.time - _startTime;
        Debug.Log($"[LevelRunStats] Level completed. Time={_completedElapsedTime:0.00}s, Cubes={_cubesUsed}, Buzzers={_buzzersTouched}.");
        LevelCompleted?.Invoke(new LevelResultData(_completedElapsedTime, _cubesUsed, _buzzersTouched));
    }

    private bool IsDebugCompletePressed()
    {
        if (Input.GetKeyDown(_debugCompleteKey))
        {
            return true;
        }

        if (Keyboard.current == null)
        {
            return false;
        }

        return _debugCompleteKey switch
        {
            KeyCode.P => Keyboard.current.pKey.wasPressedThisFrame,
            KeyCode.Space => Keyboard.current.spaceKey.wasPressedThisFrame,
            KeyCode.Return => Keyboard.current.enterKey.wasPressedThisFrame,
            _ => false
        };
    }
}

[Serializable]
public readonly struct LevelResultData
{
    public readonly float ElapsedTime;
    public readonly int CubesUsed;
    public readonly int BuzzersTouched;

    public LevelResultData(float elapsedTime, int cubesUsed, int buzzersTouched)
    {
        ElapsedTime = elapsedTime;
        CubesUsed = cubesUsed;
        BuzzersTouched = buzzersTouched;
    }
}
