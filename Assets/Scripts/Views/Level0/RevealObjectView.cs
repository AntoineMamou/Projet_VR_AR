using UnityEngine;
using Unity.Netcode;

public class RevealObjectView : NetworkBehaviour 
{
    [SerializeField] private GameObject objectToReveal;

    private NetworkVariable<bool> isObjectVisible = new NetworkVariable<bool>(
        false, // Valeur par défaut (éteint au début)
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        objectToReveal.SetActive(isObjectVisible.Value);

        isObjectVisible.OnValueChanged += (bool oldValue, bool newValue) =>
        {
            objectToReveal.SetActive(newValue);
        };
    }


    public void ShowObject()
    {
        if (IsServer) isObjectVisible.Value = true;
        else SetVisibilityRpc(true);
    }

    public void HideObject()
    {
        if (IsServer) isObjectVisible.Value = false;
        else SetVisibilityRpc(false);
    }


    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetVisibilityRpc(bool isVisible)
    {
        isObjectVisible.Value = isVisible;
    }
}