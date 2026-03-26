using Unity.Netcode;
using UnityEngine;

public class SpawningCommunicator : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        AppBootstrapper.RegisterBridge(this);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RequestSpawnRpc(string id, Vector3 position, Quaternion rotation)
    {
        Debug.Log($"[NETWORK] Server received spawn request for: {id}");

        AppBootstrapper.SpawningService.ExecuteSpawnByID(id, position, rotation);
    }
}