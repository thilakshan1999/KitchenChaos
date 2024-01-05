using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    #region VARIABLE

    [SerializeField] private KitchenObjectScriptables kitchenObjectSO;
    private FollowTranform followTranform;
    public Transform storage;
    private bool isActive;
    #endregion

    protected virtual void Awake()
    {
        followTranform = transform.GetComponent<FollowTranform>();
    }

    #region CALLBACK
    internal void TranformKitchenObjectToCarrier(IKitchenObjectCarrier kitchenObjectCarrier)
    {
        UpdateObjectPoestionToServerRpc(kitchenObjectCarrier.GetNetworkObject());
    }

    internal virtual void TranformBackToStorage()
    {
        ResetObjectPoestionToServerRpc();
    }

    internal KitchenObjectScriptables GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    internal bool GetIsActive()
    {
        return isActive;
    }

    #endregion

    #region NETCODE
    [ServerRpc(RequireOwnership = false)]
    private void UpdateObjectPoestionToServerRpc(NetworkObjectReference kictendObjectParentNetworkObjectReference)
    {
        isActive = true;
        UpdateObjectPoestionToClientRpc(kictendObjectParentNetworkObjectReference);
    }

    [ClientRpc]
    private void UpdateObjectPoestionToClientRpc(NetworkObjectReference kictendObjectParentNetworkObjectReference)
    {
        kictendObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectCarrierNetworkObject);
        IKitchenObjectCarrier kitchenObjectCarrier = kitchenObjectCarrierNetworkObject.GetComponent<IKitchenObjectCarrier>();

        followTranform.SetTargetTransform(kitchenObjectCarrier.GetKitchenObjectParent());
        kitchenObjectCarrier.SetKitchenObject(this);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetObjectPoestionToServerRpc()
    {
        isActive = false;
        ResetObjectPoestionToClientRpc();
    }

    [ClientRpc]
    private void ResetObjectPoestionToClientRpc()
    {
        followTranform.ResetTargetTranform(storage);
    }
    #endregion
}
