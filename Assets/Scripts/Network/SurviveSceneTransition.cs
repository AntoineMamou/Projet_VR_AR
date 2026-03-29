using UnityEngine;

public class SurviveSceneTransition : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}