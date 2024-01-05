using Unity.Netcode;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    #region VARIABLE
    const string GAME_SCENE = "GameScene";
    #endregion

    #region UNITY CALLBACK
    private void Awake()
    {
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if (KitchenNetworkMultiplayer.Instance != null)
        {
            Destroy(KitchenNetworkMultiplayer.Instance.gameObject);
        }
    }
    #endregion

    #region BTN FUNCTION
    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickMultiPlay()
    {
        KitchenNetworkMultiplayer.isMultiplayer = true;
        SceneLoader.LoadSceneUsingLoadingMenu(SceneLoader.Scene.LobbyScene);
    }

    public void OnClickSinglePlay()
    {
        KitchenNetworkMultiplayer.isMultiplayer = false;
        SceneLoader.LoadSceneUsingLoadingMenu(SceneLoader.Scene.GameScene);
    }
    #endregion
}
