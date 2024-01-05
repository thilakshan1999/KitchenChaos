using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameCountDownUI : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private TextMeshProUGUI countDownText;

    private Animator animator;

    private int previousNo;
    private const string NUMBER_POPUP = "NumberPopup";
    #endregion

    #region UNITY CALLBACK
    private void Start()
    {
        animator = GetComponent<Animator>();

        Hide();
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }
    private void Update()
    {
        int countDownNo = Mathf.CeilToInt(GameManager.Instance.GetCountDownTime());
        if(previousNo!= countDownNo)
        {
            previousNo = countDownNo;
            UpdateCountDown(countDownNo);
            animator.SetTrigger(NUMBER_POPUP);
            SoundManager.Instance.PlayCountDownSound();
        }
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

    private void UpdateCountDown(int countText)
    {
        countDownText.text = countText.ToString();
    }

    private void GameManager_OnGameStateChanged()
    {
        if (GameManager.Instance.gameState.Value == GameManager.GameState.COUNT_DOWN)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    #endregion
}
