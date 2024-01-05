using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryKitchenCounter : BaseKitchenCounter
{
    #region VARIABLE

    [SerializeField] private GameObject select;
    [SerializeField] private DeliveryCounterUI deliveryCounterUI;
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
        if (!player.CheckKitchenObjectIsEmpty())
        {
            if(player.GetKitchenObject() is DeliveryKitchenObject)
            {
                KitchenObject kitchenObject = player.GetKitchenObject();
                DeliveryKitchenObject deliveryKitchenObject = kitchenObject as DeliveryKitchenObject;

                DeliveryManager.Instance.RecipieDelivered(deliveryKitchenObject.GetKitchenObjectsInDeliveryObject(), OnRecipeSucess, OnRecipieFalse);

                deliveryKitchenObject.TranformBackToStorage();
                player.RemoveKitchenObject();
            }
        }
    }

    private void OnRecipeSucess()
    {
        UpdateSuceesfulEffectToServerRpc();
    }

    private void OnRecipieFalse()
    {
        UpdateFailEffectToServerRpc();
    }

    #endregion
    #region NETCODE
    [ServerRpc(RequireOwnership =false)] 
    private void UpdateSuceesfulEffectToServerRpc()
    {
        UpdateSuceesfulEffectToClientRpc();
    }

    [ClientRpc]
    private void UpdateSuceesfulEffectToClientRpc()
    {
        deliveryCounterUI.OnSucessfulDelivery();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateFailEffectToServerRpc()
    {
        UpdateFailEffectToClientRpc();
    }

    [ClientRpc]
    private void UpdateFailEffectToClientRpc()
    {
        deliveryCounterUI.OnFailDelivery();
    }
    #endregion
}
