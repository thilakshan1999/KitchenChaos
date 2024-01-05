using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseKitchenCounter : NetworkBehaviour,IKitchenCounter ,IKitchenObjectCarrier
{
    #region VARIABLE

    [Header("KITCHEN OBJECT")]
    private Transform objectParent;
    private KitchenObject kitchenObject;

    #endregion

    #region SELECTED

    public virtual void CounterDeselected()
    {
        Debug.LogError("Deselect can't get child!");
    }

    public virtual void CounterSelected()
    {
        Debug.LogError("Select can't get child!");
    }

    #endregion

    #region INTERACT
  
    public virtual void Interact(Player player)
    {
        Debug.LogError("Can't interact child!");
    }

    public virtual void AlternativeInteract(Player player)
    {
        Debug.Log("Child don't have alternate interact");
    }

    #endregion

    #region KITCHEN OBJECT
    public void SetKitchenObjectParent(Transform kitchenObjectParent)
    {
        objectParent = kitchenObjectParent;
    }
    public Transform GetKitchenObjectParent()
    {
        return objectParent;
    }

    public KitchenStorage GetKitchenStorage(KitchenObjectScriptables kitchenObjectOS, Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            KitchenStorage kitchenStorage =
                parent.GetChild(i).GetComponent<KitchenStorage>();
            if (kitchenStorage.GetKitchenObjectSO() == kitchenObjectOS)
            {
                return kitchenStorage;
            }
        }
        Debug.LogError("Cannot find kitchen Storage fpr this scrptable");
        return null;
    }

    public bool CheckKitchenObjectIsEmpty()
    {
        return kitchenObject == null;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        CheckKitchenObjectIsEmpty();
        SoundManager.Instance.PlayPickUpSound(transform.position);
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void RemoveKitchenObject()
    {
        kitchenObject = null;
        SoundManager.Instance.PlayPickUpSound(transform.position);
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public void AttachKitchenObjectWithCarrier(KitchenObject kitchenObject)
    {
        kitchenObject.TranformKitchenObjectToCarrier(this);
    }
    #endregion
}
