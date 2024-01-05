using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Lobby : MonoBehaviour
{
    #region BTN FUNCTION
    public void OnClickCreate()
    {
        KitchenNetworkMultiplayer.Instance.StartHost();
        SceneLoader.LoadNetworkScene(SceneLoader.Scene.CharacterSelectScene);
    }

    public void OnClickJoin()
    {
        KitchenNetworkMultiplayer.Instance.StartClient();
    }
    #endregion
}
