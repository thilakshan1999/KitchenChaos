using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenNetworkMultiplayer : NetworkBehaviour
{
    #region VARIABLE
    public static KitchenNetworkMultiplayer Instance { get; private set; }
    public enum PlayerCharacter
    {
        SOPHIE,
        BRAIN,
        BYRCE,
        KATE,
        MEGAN,
        REMY
    }

    private const int MAX_PLAYER = 4;

    public event Action OnTryToJoinGame;
    public event Action OnFailedToJoinGame;
    public event Action OnPlayerNetworkDataListChanged;

    internal string dissconnectMsg;

    private NetworkList<PlayerData> playerNetworkDataList;

    public static bool isMultiplayer;

    #endregion

    #region UNITY CALLBACK
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        playerNetworkDataList = new NetworkList<PlayerData>();
        playerNetworkDataList.OnListChanged += PlayerNetworkDataList_OnListChanged;

        if (!isMultiplayer)
        {
           NetworkManager.Singleton.StartHost();
        }

    }

    #endregion

    #region CREATE AND ADD PLAYERS
    internal void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApproval;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    internal void StartClient()
    {
        OnTryToJoinGame?.Invoke();
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }
    
    #endregion

    #region CONNECTION  APPROVAL
    private void NetworkManager_ConnectionApproval(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != SceneLoader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            dissconnectMsg = "Game Already Start";
            return;
        }
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER)
        {
            connectionApprovalResponse.Approved = false;
            dissconnectMsg = "Game Is Full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }
    #endregion

    #region CLINT CONNECTION
    private void NetworkManager_OnClientConnectedCallback(ulong clintId)
    {
        PlayerData newPlayerData = new PlayerData()
        {
            clientId = clintId,
            playerCharacter = (PlayerCharacter)Enum.ToObject(typeof(PlayerCharacter), playerNetworkDataList.Count)
        };

        StartCoroutine(DelayedAction(newPlayerData));
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke();
        if (SceneManager.GetActiveScene().name == SceneLoader.Scene.CharacterSelectScene.ToString())
        {
            ClearNetworkManager();
            SceneLoader.LoadSceneUsingLoadingMenu(SceneLoader.Scene.MenuScene);
        }
    }

    internal void kickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        NetworkManager_Server_OnClientDisconnectCallback(clientId);
    }

    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for(int i = 0;i < playerNetworkDataList.Count; i++){
            if(clientId == playerNetworkDataList[i].clientId)
            {
                playerNetworkDataList.RemoveAt(i);
            }
        }
    }

    #endregion

    #region OTHER FUNCTION

    private IEnumerator DelayedAction(PlayerData newPlayerData)
    {
        yield return new WaitForSeconds(0.1f);

        playerNetworkDataList.Add(newPlayerData);
    }

    private void PlayerNetworkDataList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerNetworkDataListChanged?.Invoke();
    }

    #endregion

    #region PLAYER INFO
    internal void UpdateLocalPlayerCharacter(PlayerCharacter playerCharacter)
    {
        int playerCharacterId = (int)playerCharacter;
        UpdatePlayerCharacterInPlayerDataInServerRPC(playerCharacterId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerCharacterInPlayerDataInServerRPC(int playerCharacterId, ServerRpcParams serverRpcParams = default)
    {
        int i = 0;
        foreach (PlayerData playerData in playerNetworkDataList)
        {
            if (playerData.clientId == serverRpcParams.Receive.SenderClientId)
            {
                break;
            }
            i++;
        }
        PlayerData playerDataModify = playerNetworkDataList[i];

        playerDataModify.playerCharacter = (PlayerCharacter)Enum.ToObject(typeof(PlayerCharacter), playerCharacterId);

        playerNetworkDataList[i] = playerDataModify;
    }

    internal bool IsPlayerIndexConnected(int playerIndex)
    {
        if (playerNetworkDataList.Count > playerIndex)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal PlayerData GetPlayerDataFromIndex(int playerIndex)
    {
        return playerNetworkDataList[playerIndex];
    }

    internal PlayerCharacter GetLocalPlayerCharacter()
    {
        ulong clindId = NetworkManager.Singleton.LocalClientId;
        foreach(PlayerData playerData in playerNetworkDataList)
        {
            if (playerData.clientId == clindId)
            {
                return playerData.playerCharacter;
            }
        }
        return PlayerCharacter.SOPHIE;
    }

    internal List<PlayerCharacter> GetActivePlayerCharacterList()
    {
        List<PlayerCharacter> playerCharacterList= new List<PlayerCharacter>();
        foreach (PlayerData playerData in playerNetworkDataList)
        {
            playerCharacterList.Add(playerData.playerCharacter);
        }

        return playerCharacterList;
    }

    internal int GetPlayerCharacterId(ulong clintId)
    {
        foreach (PlayerData playerData in playerNetworkDataList)
        {
            if (playerData.clientId == clintId)
            {
                return (int)playerData.playerCharacter;
            }
        }
        Debug.LogError("player character not found");
        return 0;
    }

    internal int GetPlayerId(ulong clintId)
    {
        for(int i = 0; i < playerNetworkDataList.Count; i++)
        {
            if (playerNetworkDataList[i].clientId == clintId)
            {
                return i;
            }
        }
        return 0;
    }
    #endregion

    #region CLEAR CONNECTION
    internal void ClearNetworkManager()
    {
        NetworkManager.Singleton.Shutdown();
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if (KitchenNetworkMultiplayer.Instance != null)
        {
            Destroy(KitchenNetworkMultiplayer.Instance.gameObject);
        }
    }
    #endregion

}
