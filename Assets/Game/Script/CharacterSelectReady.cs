using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectReady : NetworkBehaviour
{
    #region VARIABLE
    public static CharacterSelectReady Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary;

    [SerializeField] private Button startBtn;

    public event Action OnPlayerReady;
    #endregion

    #region UNITY CALLBACKS
    public void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    #endregion

    #region CHARACTER READY FUNCTION 
    internal void SetPlayerReady() 
    {
        UpdatePlayersReadyStatusToDictionaryToServerRpc();
    }

    internal bool IsPlayerReady(ulong clientid)
    {
        return playerReadyDictionary.ContainsKey(clientid) && playerReadyDictionary[clientid];
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayersReadyStatusToDictionaryToServerRpc(ServerRpcParams serverRpcParams = default)
    {
        bool allReady = true;
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        UpdatePlayersReadyStatusToDictionaryToClientRpc(serverRpcParams.Receive.SenderClientId);

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allReady = false;
                break;
            }
        }

        if (allReady)
        {
            startBtn.interactable = true;
        }
    }

    [ClientRpc]
    private void UpdatePlayersReadyStatusToDictionaryToClientRpc(ulong clintId)
    {
        playerReadyDictionary[clintId] = true;
        OnPlayerReady?.Invoke();
    }
    #endregion

}
