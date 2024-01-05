using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : NetworkBehaviour
{
    #region Variable
    [SerializeField] private Button readtBtn;
    [SerializeField] private Button startBtn;
    #endregion

    #region UNITY CALLBACK
    private void Start()
    {
        if (IsServer)
        {
            startBtn.gameObject.SetActive(true);
            startBtn.interactable = false;
        }
        else
        {
            startBtn.gameObject.SetActive(false);
        }
    }
    #endregion

    #region BTN FUNCTION
    public void OnClickReady()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
        readtBtn.interactable = false;
    }

    public void OnClickMainMenu()
    {
        KitchenNetworkMultiplayer.Instance.ClearNetworkManager();
        SceneLoader.LoadSceneUsingLoadingMenu(SceneLoader.Scene.MenuScene);
    }

    public void OnClickStartBtn()
    {
        SceneLoader.LoadNetworkScene(SceneLoader.Scene.GameScene);
    }
    #endregion
}
