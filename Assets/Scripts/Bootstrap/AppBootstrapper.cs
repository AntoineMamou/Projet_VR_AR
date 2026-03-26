using UnityEngine;

public class AppBootstrapper : MonoBehaviour
{
    [Header("Configurations")]
    public PlacableConfig config;

    [Header("Scene Instances")]
    [SerializeField] private TouchController _touchControllerInstance;
    [SerializeField] private MonoBehaviour _viewServiceInstance;

    public static ISpawningService SpawningService { get; private set; }
    public static SpawningCommunicator SpawningBridge { get; private set; }
    public static TouchController InstanceTouchController { get; private set; }
    public static IViewService ViewService { get; private set; }

    public static void RegisterBridge(SpawningCommunicator bridge)
    {
        SpawningBridge = bridge;
        Debug.Log("[Bootstrapper] SpawningBridge registered.");
    }
    public static void RegisterViewService(IViewService service)
    {
        ViewService = service;
        Debug.Log("[Bootstrapper] ViewService registered.");
    }

    void Awake()
    {
        SpawningService = new SpawningService(config);

        InstanceTouchController = _touchControllerInstance;

        if (_viewServiceInstance is IViewService viewService)
        {
            ViewService = viewService;
        }

        Debug.Log("[Bootstrapper] Services Initialized.");
    }
}