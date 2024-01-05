using Unity.Netcode;
using UnityEngine;

public class HostDisconnectedUI : MonoBehaviour
{
    #region UNITY CALLBACKS
    private void Start()
    {
        Hide();
        GameManager.Instance.OnHostDisconnected += GameManager_OnHostDisconnected;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnHostDisconnected -= GameManager_OnHostDisconnected;
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

    private void GameManager_OnHostDisconnected()
    {
        Show();
    }

    #endregion
}
