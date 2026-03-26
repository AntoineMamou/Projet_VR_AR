using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawningService : ISpawningService
{
    private Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();

    public SpawningService(PlacableConfig config)
    {
        foreach (var entry in config.Objects)
        {
            if (!_prefabCache.ContainsKey(entry.id))
                _prefabCache.Add(entry.id, entry.prefab);
        }
    }

    public GameObject ExecuteSpawnByID(string id, Vector3 position, Quaternion rotation)
    {
        if (_prefabCache.TryGetValue(id, out GameObject prefab))
        {
            return InternalExecuteSpawn(prefab, position, rotation);
        }

        Debug.LogWarning($"[SPAWN] ID {id} non trouvé dans le cache !");
        return null;
    }

    public GameObject ExecuteSpawnByPrefab(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return InternalExecuteSpawn(prefab, position, rotation);
    }

    private GameObject InternalExecuteSpawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!NetworkManager.Singleton.IsServer) return null;

        GameObject instance = Object.Instantiate(prefab, pos, rot);

        Transform anchor = AppBootstrapper.ViewService.GetWorldAnchor();
        if (anchor != null)
        {
            instance.transform.SetParent(anchor, true);
        }
        if (instance.TryGetComponent<NetworkObject>(out var netObj))
        {
            netObj.Spawn();
            return instance;
        }

        Debug.LogError($"[SPAWN] {prefab.name} n'a pas de NetworkObject !");
        Object.Destroy(instance);
        return null;
    }
}