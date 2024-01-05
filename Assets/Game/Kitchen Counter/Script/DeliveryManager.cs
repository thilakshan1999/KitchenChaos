using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    #region VARIABLE

    public static DeliveryManager Instance;

    [Header("RCIPIE")]
    [SerializeField] private List<RecipieScriptables> recipieList;
    private List<RecipieScriptables> toDoRecipie;
    [SerializeField] private int maxRecipiesInToDoList;

    [Header ("TIMER")]
    [SerializeField] private float maxtime;
    private float time;

    [Header("UI")]
    [SerializeField] DeliveryObjectUI deliveryObjectUI;

    private Action sucessAction;
    private Action failAction;
    #endregion

    #region UNITY CALLBACKS

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        toDoRecipie = new List<RecipieScriptables>();

    }

    private void Update()
    {
        if (!IsServer) return;

        if (GameManager.Instance.gameState.Value == GameManager.GameState.GAME_PLAYING)
        {
            Timer();
        }
    }

    #endregion

    #region FUNCTION

    private void Timer()
    {
        int recipiwScriptableIndex;
        if(toDoRecipie.Count < maxRecipiesInToDoList)
        {
            if (time <= 0)
            {
                time = maxtime;
                recipiwScriptableIndex = UnityEngine.Random.Range(0, recipieList.Count);
                UpdateDeliverObjectToClientRPC(recipiwScriptableIndex);
            }
            else
            {
                time -= Time.deltaTime;
            }
        }
       
    }

    [ClientRpc]
    private void UpdateDeliverObjectToClientRPC(int recipiwScriptableIndex)
    {
        RecipieScriptables reciipieScriptables;

        reciipieScriptables = recipieList[recipiwScriptableIndex];
        toDoRecipie.Add(reciipieScriptables);

        deliveryObjectUI.UpdateDeliverableObjectUI(toDoRecipie);
    }

    public void RecipieDelivered(List<KitchenObjectScriptables> kitchenObjectsInDeliveryObject,Action sucessAction,Action failAction)
    {
        bool hasIngredient = false;
        bool hasRecipie;

        this.sucessAction = sucessAction;
        this.failAction = failAction;

        foreach (RecipieScriptables recipie in toDoRecipie)
        {
            if(recipie.ingredientsList.Count == kitchenObjectsInDeliveryObject.Count)
            {
                hasRecipie = true;

                foreach(KitchenObjectScriptables kitchenObjectSOInRecipie in recipie.ingredientsList)
                {
                    foreach(KitchenObjectScriptables kitchenObjectSOInDeliveryObject in kitchenObjectsInDeliveryObject)
                    {
                        hasIngredient = false;
                        if (kitchenObjectSOInRecipie == kitchenObjectSOInDeliveryObject)
                        {
                            hasIngredient = true;
                            break;
                        }
                    }
                    if (!hasIngredient)
                    {
                        //No mathch found;
                        hasRecipie = false;
                        break;
                    }
                }

                if(hasRecipie == true)
                {
                    // match found
                    int recipieIndex = toDoRecipie.FindIndex(r => r == recipie);
                    UpdateSucessfullDeliveryToServerRpc(recipieIndex);
                    return;
                }

            }
        }

        //No mathch found;
        UpdateFaillDeliveryToServerRpc();
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateSucessfullDeliveryToServerRpc(int recipieIndex)
    {
        UpdateSucessfullDeliveryToClientRpc(recipieIndex);
        GameManager.Instance.IncreseDeliverdRecipie();
    }

    [ClientRpc]
    private void UpdateSucessfullDeliveryToClientRpc(int recipieIndex)
    {
        SoundManager.Instance.PlayDeliverySucessSound();
        toDoRecipie.RemoveAt(recipieIndex);
        deliveryObjectUI.UpdateDeliverableObjectUI(toDoRecipie);
        sucessAction?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateFaillDeliveryToServerRpc()
    {
        UpdateFailDeliveryToClientRpc();
    }

    [ClientRpc]
    private void UpdateFailDeliveryToClientRpc()
    {
        SoundManager.Instance.PlayDeliveryFailSound();
        failAction?.Invoke();
    }
    #endregion
}

