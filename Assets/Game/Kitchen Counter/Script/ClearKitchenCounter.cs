using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClearKitchenCounter : BaseKitchenCounter
{
    #region VARIABLE

    [SerializeField] private GameObject select;

    [Header("KITCHEN OBJECT")]
    [SerializeField] private Transform kitchenObjectParent;

    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        SetKitchenObjectParent(kitchenObjectParent);
    }
    #endregion

    #region SELECTED

    public override void CounterSelected()
    {
        select.gameObject.SetActive(true);
    }

    public override void CounterDeselected()
    {
        select.gameObject.SetActive(false);
    }

    #endregion

    #region INTERACT

    public override void Interact(Player player)
    {
        if (player.CheckKitchenObjectIsEmpty())
        {
            if (CheckKitchenObjectIsEmpty())
            {
                //player and counter in null
            }
            else
            {
                //conter has object, player is null
                player.AttachKitchenObjectWithCarrier(GetKitchenObject());
                UpdateRemoveObjectToServerRpc();
            }
        }
        else
        {
            if (CheckKitchenObjectIsEmpty())
            {
                //player has object , couter is null
                AttachKitchenObjectWithCarrier(player.GetKitchenObject());
                player.RemoveKitchenObject();
            }
            else
            {
                //player and counter both have object

                //player has delvery object and couter has accept object
                if (player.GetKitchenObject() is DeliveryKitchenObject)
                {
                    DeliveryKitchenObject deliveryKitchenObject = player.GetKitchenObject() as DeliveryKitchenObject;
                    if ( deliveryKitchenObject.CheckKitchenObjectIsDeleliverable(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        if (deliveryKitchenObject.TryToAddKitchenObjectToDeliveryObjectList(GetKitchenObject()))
                        {
                            GetKitchenObject().TranformBackToStorage();
                            UpdateRemoveObjectToServerRpc();
                        }
                        
                    }
                }

                //Couter has deliver object and player has accept object
                else if (GetKitchenObject() is DeliveryKitchenObject)
                {
                    DeliveryKitchenObject deliveryKitchenObject = GetKitchenObject() as DeliveryKitchenObject;
                    if (deliveryKitchenObject.CheckKitchenObjectIsDeleliverable(player.GetKitchenObject().GetKitchenObjectSO()))
                    {
                        if (deliveryKitchenObject.TryToAddKitchenObjectToDeliveryObjectList(player.GetKitchenObject()))
                        {
                            player.GetKitchenObject().TranformBackToStorage();
                            player.RemoveKitchenObject();
                            UpdateEffectToServerRpc();
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region NETCODE
    [ServerRpc(RequireOwnership =false)]
    private void UpdateRemoveObjectToServerRpc()
    {
        UpdateRemoveObjectToClientRpc();
    }

    [ClientRpc]
    private void UpdateRemoveObjectToClientRpc()
    {
        RemoveKitchenObject();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateEffectToServerRpc()
    {
        UpdateEffectToClientRpc();
    }

    [ClientRpc]
    private void UpdateEffectToClientRpc()
    {
        SoundManager.Instance.PlayPickUpSound(transform.position);
    }
    #endregion

}
