using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashKitchenCounter : BaseKitchenCounter
{
    #region VARIABLE
    [SerializeField] private GameObject select;
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
            KitchenObject kitchenObject = player.GetKitchenObject();
            kitchenObject.TranformBackToStorage();
            player.RemoveKitchenObject();
            UpdateTrashEffectToServerRpc();
        }
    }

    #endregion

    #region NETCODE
    [ServerRpc(RequireOwnership = false)]
    private void UpdateTrashEffectToServerRpc()
    {
        UpdateTrashEffectToClientRpc();
    }

    [ClientRpc]
    private void UpdateTrashEffectToClientRpc()
    {
        SoundManager.Instance.PlayTrashSound();
    }
    #endregion
}
