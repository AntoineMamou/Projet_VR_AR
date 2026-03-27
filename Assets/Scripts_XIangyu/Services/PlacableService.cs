using System.Collections.Generic;
using UnityEngine;
public class PlacableService : IPlacableService
{
    private Dictionary<string, GameObject> _prefabCache = new Dictionary<string, GameObject>();

    public PlacableService(PlacableConfig config)
    {
        foreach (var entry in config.Objects)
        {
            if (!_prefabCache.ContainsKey(entry.id))
            {
                _prefabCache.Add(entry.id, entry.prefab);
            }
        }
    }

    public GameObject SpawnObject(string id, Vector3 position, Quaternion rotation)
    {
        if (_prefabCache.TryGetValue(id, out GameObject prefab))
        {
            GameObject instance = Object.Instantiate(prefab, position, rotation);

            if (instance.TryGetComponent<IPlacable>(out var placable))
            {
                placable.Initialize(id);
                placable.OnPlaced(position, rotation);
            }
            return instance;
        }

        Debug.LogWarning($"Objet avec ID {id} non trouvé !");
        return null;
    }
}