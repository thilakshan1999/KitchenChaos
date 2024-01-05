using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ConnectineResponseUI : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private TextMeshProUGUI message; 
    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        KitchenNetworkMultiplayer.Instance.OnFailedToJoinGame += KitchenNetworkMultiplayer_OnFailedToJoinGame;
        Hide();
    }

    private void OnDestroy()
    {
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

    private void KitchenNetworkMultiplayer_OnFailedToJoinGame()
    {
        Show();

        message.text = KitchenNetworkMultiplayer.Instance.dissconnectMsg;

        if (KitchenNetworkMultiplayer.Instance.dissconnectMsg == null)
        {
            message.text = "Failed to connect";
        }
    }
    #endregion

    #region BTN FUNCTION
    public void OnClickCancel()
    {
        Hide();
    }
    #endregion
}
