using Unity.Netcode;
using UnityEngine;
public class AROnlyConnection : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Scene configuration scriptable object")]
    public SceneConfig sceneConfig;

    [Header("World Spawning")]
    [Tooltip("Reference to WorldSpawner component")]
    public WorldSpawner worldSpawner;

    void Start()
    {
        InitializeARTesting();
    }

    private void InitializeARTesting()
    {
        if (sceneConfig == null)
        {
            Debug.LogError("[AR TEST] SceneConfig non assigné !");
            return;
        }

        if (sceneConfig.IsARScene())
        {
            Debug.Log("[AR TEST] Mode test AR activé - Contournement du réseau");
            TriggerWorldSpawning();
        }
        else
        {
            Debug.LogWarning($"[AR TEST] Script AR-only utilisé dans une scène non-AR : '{sceneConfig.GetCurrentSceneName()}'");
            Debug.Log("[AR TEST] Utilisation d'AsymmetricConnection recommandée pour les scènes VR");
        }

        if (sceneConfig.IsARScene())
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                NetworkManager.Singleton.StartHost();
                Debug.Log("[AR TEST] Starting Local Host to satisfy NetworkTransforms.");
            }

            TriggerWorldSpawning();
        }
        else
        {
            Debug.LogWarning($"[AR TEST] Script AR-only utilisé dans une scène non-AR : '{sceneConfig.GetCurrentSceneName()}'");
        }
    }
    private void TriggerWorldSpawning()
    {
        if (worldSpawner != null)
        {
            Debug.Log("[AR TEST] Démarrage du spawning des objets du monde");
            worldSpawner.StartSpawning();
        }
        else
        {
            Debug.LogWarning("[AR TEST] Référence WorldSpawner est null !");
        }
    }



}