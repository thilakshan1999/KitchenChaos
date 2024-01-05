using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaitinUI : MonoBehaviour
{
    #region VARIABLE

    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        Show();
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
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
        if (GameManager.Instance.gameState.Value == GameManager.GameState.COUNT_DOWN)
        {
            Hide();
        }
    }
    #endregion
}
