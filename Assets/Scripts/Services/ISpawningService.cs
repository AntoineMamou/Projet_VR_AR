using UnityEngine;

public interface ISpawningService
{
    GameObject ExecuteSpawnByID(string id, Vector3 position, Quaternion rotation);
    GameObject ExecuteSpawnByPrefab(GameObject prefab, Vector3 position, Quaternion rotation);
}