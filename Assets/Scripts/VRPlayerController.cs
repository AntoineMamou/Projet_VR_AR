using Unity.Netcode;
using UnityEngine;

[DisallowMultipleComponent]
public class VRPlayerController : NetworkBehaviour
{
    [SerializeField] private GameObject[] _localOnlyObjects;
    [SerializeField] private Behaviour[] _localOnlyBehaviours;
    [SerializeField] private Camera[] _localOnlyCameras;
    [SerializeField] private AudioListener[] _localOnlyAudioListeners;

    public override void OnNetworkSpawn()
    {
        ApplyOwnershipState();
    }

    private void Start()
    {
        if (!IsSpawned)
        {
            ApplyOwnershipState();
        }
    }

    private void ApplyOwnershipState()
    {
        bool isLocalPlayer = IsOwner || !IsSpawned;

        SetObjectsActive(_localOnlyObjects, isLocalPlayer);
        SetBehavioursEnabled(_localOnlyBehaviours, isLocalPlayer);
        SetBehavioursEnabled(_localOnlyCameras, isLocalPlayer);
        SetBehavioursEnabled(_localOnlyAudioListeners, isLocalPlayer);
    }

    private void SetObjectsActive(GameObject[] objects, bool isActive)
    {
        foreach (var targetObject in objects)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(isActive);
            }
        }
    }

    private void SetBehavioursEnabled(Behaviour[] behaviours, bool isEnabled)
    {
        foreach (var behaviour in behaviours)
        {
            if (behaviour != null)
            {
                behaviour.enabled = isEnabled;
            }
        }
    }
}
