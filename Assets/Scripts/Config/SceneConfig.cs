using UnityEngine;

[CreateAssetMenu(fileName = "SceneConfig", menuName = "Config/Scene Configuration")]
public class SceneConfig : ScriptableObject
{
    [Header("Scene Names")]
    [Tooltip("Exact name of the AR Scene")]
    public string arSceneName = "ArScene";

    [Tooltip("Exact name of the VR Scene")]
    public string vrSceneName = "VrScene";
    private string _currentSceneName;

    public enum SceneType
    {
        AR,
        VR,
        Unknown
    }

    public SceneType GetCurrentSceneType()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        _currentSceneName = currentScene;

        if (currentScene == arSceneName)
        {
            return SceneType.AR;
        }
        else if (currentScene == vrSceneName)
        {
            return SceneType.VR;
        }
        else
        {
            return SceneType.Unknown;
        }
    }
    public bool IsARScene() => GetCurrentSceneType() == SceneType.AR;

    public bool IsVRScene() => GetCurrentSceneType() == SceneType.VR;

    public string GetCurrentSceneName() => _currentSceneName;
}