using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectCarrier
{
    #region KITCHEN OBJECT

    public bool CheckKitchenObjectIsEmpty();

    public void SetKitchenObject(KitchenObject kitchenObject);

    public KitchenObject GetKitchenObject();

    public void RemoveKitchenObject();

    public NetworkObject GetNetworkObject();

    public Transform GetKitchenObjectParent();

    public void AttachKitchenObjectWithCarrier(KitchenObject kitchenObject);
    #endregion
}
