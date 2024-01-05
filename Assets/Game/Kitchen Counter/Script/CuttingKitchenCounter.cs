using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CuttingKitchenCounter : BaseKitchenCounter
{
    #region VARIABLE

    [SerializeField] private GameObject select;

    [Header("KITCHEN OBJECT")]
    [SerializeField] private Transform kitchenStoreParent;
    [SerializeField] private Transform kitchenObjectParent;
    [SerializeField] private List<CuttingObjectScriptable> cuttingObjectsSOList;
    private CuttingObjectScriptable cuttingObjectSO;
    private int cuttingCount;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private const string CUT = "Cut";

    [Header("UI")]
    [SerializeField] private GameObject progressBar;
    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        SetKitchenObjectParent(kitchenObjectParent);
        DeactiveProgressBar();
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
                if (!CheckCuttingObjectSOIsEmpty(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //player has cuttable object
                    AttachKitchenObjectWithCarrier(player.GetKitchenObject());
                    player.RemoveKitchenObject();
                }
                else
                {
                    //player has not have cuttable object;
                }
            }
           
            else
            {
                //player and counter both have object
                if (player.GetKitchenObject() is DeliveryKitchenObject)
                {
                    DeliveryKitchenObject deliveryKitchenObject = player.GetKitchenObject() as DeliveryKitchenObject;
                    if (deliveryKitchenObject.CheckKitchenObjectIsDeleliverable(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        if (deliveryKitchenObject.TryToAddKitchenObjectToDeliveryObjectList(GetKitchenObject()))
                        {
                            GetKitchenObject().TranformBackToStorage();
                            UpdateRemoveObjectToServerRpc();
                        }

                    }
                }
            }
        }
    }

    public override void AlternativeInteract(Player player)
    {
        if (!CheckKitchenObjectIsEmpty())
        {
            if (cuttingObjectSO.inputKitchenObjectSO == GetKitchenObject().GetKitchenObjectSO())
            {
                UpdateProgressToServerRpc();
            }
            else
            {
                //Already cut;
            }
        }
        else
        {
            //Kitchen object is empty
        }
    }
    #endregion

    #region KITCHEN OBJECT

    private bool CheckCuttingObjectSOIsEmpty(KitchenObjectScriptables kitchenObjectSO)
    {
        foreach(CuttingObjectScriptable cuttingObjectSO in cuttingObjectsSOList)
        {
            if (cuttingObjectSO.inputKitchenObjectSO == kitchenObjectSO)
            {
                this.cuttingObjectSO = cuttingObjectSO;
            }
        }
        return cuttingObjectSO == null;
    }

    private void GetOutputKitchenObject()
    {
        KitchenObjectScriptables outputKitchenObjectSo = cuttingObjectSO.outputKitchenObjectSO;
        KitchenStorage kitchenStorage = GetKitchenStorage(outputKitchenObjectSo, kitchenStoreParent);
        kitchenStorage.GetKitchenObject(this);
    }

    #endregion

    #region UI
    private void ActiveProgressBar()
    {
        if (!progressBar.activeSelf)
        {
            progressBar.SetActive(true);
        }
    }
    private void DeactiveProgressBar()
    {
        if (progressBar.activeSelf)
        {
            progressBar.SetActive(false);
            progressBar.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
        }
    }

    private void IncreseProgress(float cuttingCount, float maxCuttingCount)
    {
            progressBar.transform.GetChild(2).GetComponent<Image>().fillAmount = cuttingCount / maxCuttingCount;
    }
    #endregion

    #region NETCODE
    [ServerRpc(RequireOwnership =false)]
    private void UpdateRemoveObjectToServerRpc()
    {
        cuttingCount = 0;
        UpdateRemoveObjectToClientRpc();
        UpdateDeactivateProgressbarToClientRpc();
    }

    [ClientRpc]
    private void UpdateRemoveObjectToClientRpc()
    {
        cuttingObjectSO = null;
        RemoveKitchenObject();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateProgressToServerRpc()
    {
        if (!CheckCuttingObjectSOIsEmpty(GetKitchenObject().GetKitchenObjectSO()))
        {
            if (cuttingCount >= cuttingObjectSO.cuttingCount - 1)
            {
                //cutting end
                cuttingCount = 0;
                GetKitchenObject().TranformBackToStorage();
                GetOutputKitchenObject();
                UpdateDeactivateProgressbarToClientRpc();
            }
            else
            {
                //cutting progress
                cuttingCount++;
                UpdateProgressbarProgressToClientRpc(cuttingCount, cuttingObjectSO.cuttingCount);
            }
        }
        
        UpdateEffectToClientRpc();
    }


    [ClientRpc]
    private void UpdateEffectToClientRpc()
    {
        animator.SetTrigger(CUT);
        SoundManager.Instance.PlayChopSound();
    }

    [ClientRpc]
    private void UpdateDeactivateProgressbarToClientRpc()
    {
        DeactiveProgressBar();
    }

    [ClientRpc]
    private void UpdateProgressbarProgressToClientRpc(float cuttingCount, float maxCuttingCount)
    {
        ActiveProgressBar();
        IncreseProgress(cuttingCount,maxCuttingCount);
    }
    #endregion

}
