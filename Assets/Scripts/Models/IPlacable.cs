using UnityEngine;
public interface IPlacable
{
    void Initialize(string id);
    void OnPlaced(Vector3 position, Quaternion rotation);
}