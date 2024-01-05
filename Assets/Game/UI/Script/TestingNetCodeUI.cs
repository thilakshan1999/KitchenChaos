using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestingNetCodeUI : MonoBehaviour
{
    #region VARIABLE
    #endregion

    #region BTN FUNCTION
    public void OnClickHost()
    {
        NetworkManager.Singleton.StartHost();
        Hide();
    }

    public void OnClickClient()
    {
        NetworkManager.Singleton.StartClient();
        Hide();
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
    #endregion
}
