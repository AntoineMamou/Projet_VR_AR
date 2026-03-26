using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class WorldSpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [Tooltip("List of structural prefabs (walls, floor, etc.)")]
    public List<GameObject> objectsToSpawn = new List<GameObject>();

    private bool hasSpawned = false;
    public void StartSpawning()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (hasSpawned) return;

        SpawnAllObjects();
        hasSpawned = true;
    }

    private void SpawnAllObjects()
    {
        Debug.Log($"[SPAWN] Initialisation du niveau ({objectsToSpawn.Count} objets)");

        foreach (GameObject prefab in objectsToSpawn)
        {
            if (prefab == null) continue;

            AppBootstrapper.SpawningService.ExecuteSpawnByPrefab(
                prefab,
                prefab.transform.position,
                prefab.transform.rotation
            );
        }
    }
}