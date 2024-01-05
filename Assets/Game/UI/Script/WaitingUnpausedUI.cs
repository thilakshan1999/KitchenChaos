using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingUnpausedUI : MonoBehaviour
{
    #region UNITY CALLBACKS
    private void Start()
    {
        Hide();
        GameManager.Instance.OnShowWaitPauseUI += GameManager_OnShowWaitPauseUI;
        GameManager.Instance.OnHideWaitPauseUI += GameManager_OnHideWaitPauseUI;
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
    private void GameManager_OnHideWaitPauseUI()
    {
        Hide();
    }

    private void GameManager_OnShowWaitPauseUI()
    {
        Show();
    }
    #endregion
}
