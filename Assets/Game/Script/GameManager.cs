using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    #region VARIABLE
    public static GameManager Instance{ get; private set; }

    public event Action OnGameStateChanged;
    public event Action OnLocalPlayerInteract;
    public event Action OnDeliverCountChanged;
    public event Action OnShowPauseUI;
    public event Action OnHidePauseUI;
    public event Action OnShowWaitPauseUI;
    public event Action OnHideWaitPauseUI;
    public event Action OnHostDisconnected;
    public event Action OnToggleSound;

    public enum GameState
    {
        GAME_INIT,
        COUNT_DOWN,
        GAME_PLAYING,
        GAME_END
    }
    public NetworkVariable<GameState> gameState { get; private set; } = new NetworkVariable<GameState>(GameState.GAME_INIT);

    private NetworkVariable<float> countDownTime = new NetworkVariable<float>(3f);
    private NetworkVariable<float> playingTime = new NetworkVariable<float>(60f);

    private NetworkVariable<int> deliverdRecipieCount = new NetworkVariable<int>(0);
    [SerializeField]private float increseTimePerDelivery=30f;

    private bool isLocalGamePaused;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);

    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPauseDictionary;

    private bool AutoReusme = false;

    [SerializeField] private List<Transform> playerPrefebs;

    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPauseDictionary = new Dictionary<ulong, bool>();
        
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        GameInput.Instance.OnInteractionAction += GameInput_OnInteractionAction;
    }

    private void Update()
    {
        if (!IsServer) return;
        SwitchGameState();
    }

    private void LateUpdate()
    {
        if (!AutoReusme) return;
        CheckAndUpdateGamePaused();
        AutoReusme = false;
    }

    #endregion

    #region GAME STATE
    private void GameState_OnValueChanged(GameState previousValue, GameState newValue)
    {
        OnGameStateChanged?.Invoke();
        if (gameState.Value == GameState.GAME_END)
        {
            StartCoroutine(PauseDelay());
        }
    }

    private void SwitchGameState()
    {
        switch (gameState.Value)
        {
            case GameState.GAME_INIT:
                break;
            case GameState.COUNT_DOWN:
                countDownTime.Value -= Time.deltaTime;
                if (countDownTime.Value <= 0)
                {
                    gameState.Value = GameState.GAME_PLAYING;
                }
                break;
            case GameState.GAME_PLAYING:
                playingTime.Value -= Time.deltaTime;
                if (playingTime.Value <= 0)
                {
                    gameState.Value = GameState.GAME_END;
                }
                break;
            case GameState.GAME_END:
                break;
        }
    }

    private void GameInput_OnInteractionAction(object sender, System.EventArgs e)
    {
        if (gameState.Value == GameState.GAME_INIT)
        {
            
            OnLocalPlayerInteract?.Invoke();
            UpdatePlayersReadyStatusToDictionaryToServerRpc();
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void UpdatePlayersReadyStatusToDictionaryToServerRpc(ServerRpcParams serverRpcParams = default)
    {
        bool allReady = true;

        playerReadyDictionary.Add(serverRpcParams.Receive.SenderClientId, true);

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
            gameState.Value = GameState.COUNT_DOWN;
        }
    }

    internal float GetCountDownTime()
    {
        return countDownTime.Value;
    }

    internal float GetPlayingTime()
    {
        return playingTime.Value;
    }

    #endregion

    #region DELIVERY RECIPIE
    internal int GetDeliverdRecipieCount()
    {
        return deliverdRecipieCount.Value;
    }

    internal void IncreseDeliverdRecipie()
    {
        deliverdRecipieCount.Value++;
        playingTime.Value += increseTimePerDelivery;
    }
    private void DeliveryCount_OnValueChanged(int previousValue, int newValue)
    {
        OnDeliverCountChanged();
    }
    #endregion

    #region GAME PAUSE
    private void GamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            StartCoroutine(PauseDelay());
            if (!isLocalGamePaused)
            {
                OnShowWaitPauseUI?.Invoke();
            }
        }
        else
        {
            Time.timeScale = 1;
            OnHideWaitPauseUI?.Invoke();
        }
        OnToggleSound?.Invoke();
    }

    private IEnumerator PauseDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Time.timeScale = 0;
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e)
    {
        if(gameState.Value == GameState.GAME_PLAYING)
        {
            TogglePause();
        }
    }

    internal void TogglePause()
    {
        if (isLocalGamePaused)
        {
            isLocalGamePaused = false;
            UpdateGameUnpauseToServerRpc();
            if (isGamePaused.Value)
            {
                OnShowWaitPauseUI?.Invoke();
            }
            OnHidePauseUI?.Invoke();
        }
        else
        {
            isLocalGamePaused = true;
            UpdateGamePauseToServerRpc();
            OnShowPauseUI?.Invoke();
            if (isGamePaused.Value)
            {
                OnHideWaitPauseUI?.Invoke();
            }
        }
    }

    internal bool GetIsGamePaused()
    {
        return isGamePaused.Value;
    }

    [ServerRpc(RequireOwnership =false)]
    private void UpdateGamePauseToServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;
        CheckAndUpdateGamePaused();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateGameUnpauseToServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;
        CheckAndUpdateGamePaused();
    }

    private void CheckAndUpdateGamePaused()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId])
            {
                isGamePaused.Value = true;
                return;
            }
        }
        isGamePaused.Value = false;
    }
    #endregion

    #region NETCODE
    public override void OnNetworkSpawn()
    {
        gameState.OnValueChanged += GameState_OnValueChanged;
        isGamePaused.OnValueChanged += GamePaused_OnValueChanged;
        deliverdRecipieCount.OnValueChanged += DeliveryCount_OnValueChanged;

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            OnHostDisconnected?.Invoke();
        }
        else
        {
            AutoReusme = true;
        }

    }
    #endregion

    #region PLAYER 
    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        int i = 0;
        if (!IsServer) return;
        if (KitchenNetworkMultiplayer.isMultiplayer)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Transform playerTranform = Instantiate(playerPrefebs[KitchenNetworkMultiplayer.Instance.GetPlayerCharacterId(clientId)]);
                playerTranform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                i++;
            }
        }
        else
        {
            Transform playerTranform = Instantiate(playerPrefebs[0]);
            playerTranform.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.ConnectedClientsIds[0], true);
        }
        
    }
    #endregion
}
