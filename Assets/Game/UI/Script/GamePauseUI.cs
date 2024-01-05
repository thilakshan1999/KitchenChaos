using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GamePauseUI : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private GameOptionUI gameOptionUI;
    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        Hide();
        GameManager.Instance.OnShowPauseUI += GameManager_OnShowPauseUI;
        GameManager.Instance.OnHidePauseUI += GameManager_OnHidePauseUI;
    }

    #endregion

    #region BTN FUNCTION
    public void OnClickResume()
    {
        GameManager.Instance.TogglePause();
        Hide();
    }

    public void OnClickOption()
    {
        gameOptionUI.OpenOptionMenu(Show);
    }

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

    private void GameManager_OnHidePauseUI()
    {
        Hide();
    }

    private void GameManager_OnShowPauseUI()
    {
        Show();
    }
    #endregion
}
