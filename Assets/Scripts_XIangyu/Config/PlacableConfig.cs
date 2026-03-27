using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlacableConfig", menuName = "Config/PlacableObjects")]
public class PlacableConfig : ScriptableObject
{
    [SerializeField] private List<PlacableEntry> _objects;
    public IReadOnlyList<PlacableEntry> Objects => _objects;


    [System.Serializable]
    public struct PlacableEntry
    {
        public string id;
        public GameObject prefab;
        public Sprite icon;
    }
}
