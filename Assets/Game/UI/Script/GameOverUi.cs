using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameOverUi : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private TextMeshProUGUI recipieCountText;
    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        Hide();
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }


    #endregion

    #region BTN FUNCTION
    public void OnClickRetry()
    {
        if (KitchenNetworkMultiplayer.isMultiplayer)
        {
            KitchenNetworkMultiplayer.Instance.ClearNetworkManager();
            SceneLoader.LoadSceneUsingLoadingMenu(SceneLoader.Scene.LobbyScene);
        }
        else
        {
            KitchenNetworkMultiplayer.Instance.ClearNetworkManager();
            SceneLoader.LoadSceneUsingLoadingMenu(SceneLoader.Scene.GameScene);
        }
    }

    public void OnClickMenu()
    {
        KitchenNetworkMultiplayer.Instance.ClearNetworkManager();
        SceneLoader.LoadSceneUsingLoadingMenu(SceneLoader.Scene.MenuScene);
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

    private void GameManager_OnGameStateChanged()
    {
        if (GameManager.Instance.gameState.Value == GameManager.GameState.GAME_END)
        {
            UpdateRecipieCount();
            Show();
        }
    }

    private void UpdateRecipieCount()
    {
        recipieCountText.text = GameManager.Instance.GetDeliverdRecipieCount().ToString();
    }
    #endregion
}
