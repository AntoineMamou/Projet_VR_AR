using UnityEngine;
public interface IPlacableService
{
    GameObject SpawnObject(string id, Vector3 position, Quaternion rotation);
}
