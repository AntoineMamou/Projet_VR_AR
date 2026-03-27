using UnityEngine;

public class AppBootstrapper : MonoBehaviour
{
    public PlacableConfig config;
    public static IPlacableService PlacableService { get; private set; }
    public static TouchController TouchController { get; private set; }

    [SerializeField] private TouchController _touchControllerInstance;

    void Awake()
    {
        PlacableService = new PlacableService(config);
        TouchController = _touchControllerInstance;
    }
}