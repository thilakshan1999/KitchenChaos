using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenCounter : BaseKitchenCounter
{
    #region VARIABLE
    [SerializeField] private GameObject select;

    [SerializeField] private KitchenObjectScriptables kitchenObjectSO;

    [SerializeField] private Transform kitchenStorageParent;
    private KitchenStorage kitchenStorage;

    [SerializeField] private Transform kitchenObjectParent;

    [SerializeField] private int maxPlate;
    private int plateCount;

    [SerializeField] private float spawmTime;
    private float time;

    private List<GameObject> plateList;

    private float plateGap = 0.1f;
    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        time = spawmTime;
        kitchenStorage = GetKitchenStorage(kitchenObjectSO, kitchenStorageParent);

        CreateVisual();
    }

    private void Update()
    {
        if (!IsServer) return;
        TimerFunction();
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
            if (plateCount != 0)
            {
                kitchenStorage.GetKitchenObject(player);
                UpdateRemovePlateToServerRpc();
            }
            
        }
    }

    #endregion

    #region TIMER
    private void TimerFunction()
    {
        if (plateCount >= maxPlate ||GameManager.Instance.gameState.Value != GameManager.GameState.GAME_PLAYING)
        {
            return;
        }
        else
        {
            if (time >= spawmTime)
            {
                UpdateAddPlateToClientRpc();
                time = 0;
            }
            else
            {
                time += Time.deltaTime;
            }
        }
    }

    #endregion

    #region VISUAL
    private void CreateVisual()
    {
        plateList = new List<GameObject>();

        for(int i = 0; i < maxPlate; i++)
        {
            GameObject plate = Instantiate(kitchenObjectSO.kitchenObject, kitchenObjectParent);
            plate.transform.localPosition = Vector3.up * i * plateGap;
            plate.transform.localScale = Vector3.one*1.7f;
            plate.gameObject.SetActive(false);
            plateList.Add(plate);
        }
    }
    private void AddPlate()
    {
        plateList[plateCount].gameObject.SetActive(true);
        plateCount++;
    }

    private void RemovePlate()
    {
        plateCount--;
        plateList[plateCount].gameObject.SetActive(false);
        
    }
    #endregion

    #region NETCODE
    [ClientRpc]
    private void UpdateAddPlateToClientRpc()
    {
        AddPlate();
    }

    [ServerRpc(RequireOwnership =false)]
    private void UpdateRemovePlateToServerRpc()
    {
        UpdateRemovePlateToClientRpc();
    }

    [ClientRpc]
    private void UpdateRemovePlateToClientRpc()
    {
        RemovePlate();
        SoundManager.Instance.PlayPickUpSound(transform.position);
    }
    #endregion
}
