using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    #region UNITY CALLBACKS
    private void Start()
    {
        KitchenNetworkMultiplayer.Instance.OnTryToJoinGame += KitchenNetworkMultiplayer_OnTryToJoinGame;
        KitchenNetworkMultiplayer.Instance.OnFailedToJoinGame += KitchenNetworkMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void OnDestroy()
    {
        KitchenNetworkMultiplayer.Instance.OnTryToJoinGame -= KitchenNetworkMultiplayer_OnTryToJoinGame;
        KitchenNetworkMultiplayer.Instance.OnFailedToJoinGame -= KitchenNetworkMultiplayer_OnFailedToJoinGame;
    }
    #endregion

    #region UI FUNCTION
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void KitchenNetworkMultiplayer_OnTryToJoinGame()
    {
        Show();
    }
    private void KitchenNetworkMultiplayer_OnFailedToJoinGame()
    {
        Hide();
    }
    #endregion

}
