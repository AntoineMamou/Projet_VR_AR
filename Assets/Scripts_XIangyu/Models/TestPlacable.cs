using UnityEngine;
public class TestPlacable : MonoBehaviour, IPlacable
{


    private string _storedId;
    public void Initialize(string id)
    {
        _storedId = id;
        Debug.Log($"Initialisé avec ID: {_storedId}");
    }

    public void OnPlaced(Vector3 pos, Quaternion rot)
    {
        Debug.Log($"[Objet: {_storedId}] Placé à la position : {pos} avec la rotation : {rot.eulerAngles}");
    }
}