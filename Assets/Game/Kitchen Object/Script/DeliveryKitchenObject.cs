using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryKitchenObject : KitchenObject
{
    #region VARIABLE

    [SerializeField] private List<KitchenObjectScriptables> deiverableKitchenObjects;
    private List<KitchenObjectScriptables> kitchenObjectsInDeliveryObject;

    [SerializeField] private PlateVisual plateVisual;

    #endregion

    #region UNITY CALLBAKS
    protected override void Awake()
    {
        base.Awake();
        kitchenObjectsInDeliveryObject = new List<KitchenObjectScriptables>();
    }
    #endregion

    #region OBJECTS

    internal bool CheckKitchenObjectIsDeleliverable(KitchenObjectScriptables kitchenObjectSO)
    {
        return deiverableKitchenObjects.Contains(kitchenObjectSO);
    }

    internal bool TryToAddKitchenObjectToDeliveryObjectList(KitchenObject kitchenObject)
    {
        if (kitchenObjectsInDeliveryObject.Contains(kitchenObject.GetKitchenObjectSO())) 
        {
            return false;
        }
        else
        {
            UpdateAddingKitchenObjectToServerRpc(kitchenObject.NetworkObject);
            return true;
        }
    }

    internal override void TranformBackToStorage()
    {
        base.TranformBackToStorage();
        ResetVisualsToServerRpc();
    }

    internal List<KitchenObjectScriptables> GetKitchenObjectsInDeliveryObject()
    {
        return kitchenObjectsInDeliveryObject;
    }

    #endregion

    #region NETCODE
    [ServerRpc(RequireOwnership = false)]
    private void UpdateAddingKitchenObjectToServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        UpdateAddingKitchenObjectToClientRpc(kitchenObjectNetworkObjectReference);
    }

    [ClientRpc]
    private void UpdateAddingKitchenObjectToClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenNetworkObject);
        KitchenObject kitchenObject = kitchenNetworkObject.GetComponent<KitchenObject>();

        kitchenObjectsInDeliveryObject.Add(kitchenObject.GetKitchenObjectSO());
        plateVisual.EnableVisual(kitchenObject.GetKitchenObjectSO());
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetVisualsToServerRpc()
    {
        ResetVisualsToClientRpc();
    }

    [ClientRpc]
    private void ResetVisualsToClientRpc()
    {
        plateVisual.DisableAllVisuals();
        kitchenObjectsInDeliveryObject.Clear();
    }

    #endregion
}
