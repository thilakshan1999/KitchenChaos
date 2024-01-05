using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private TextMeshProUGUI TimeText;

    private int previousNo;
    #endregion

    #region UNITY CALLBACK
    private void Start()
    {

    }
    private void Update()
    {
        int countDownNo = Mathf.CeilToInt(GameManager.Instance.GetPlayingTime());
        if (previousNo != countDownNo)
        {
            previousNo = countDownNo;
            UpdateCountDown(countDownNo);
        }
    }
    #endregion

    #region UI FUNCTION
    private void UpdateCountDown(int time)
    {
        int minute = time / 60;
        int second = time % 60;
        if(minute == 0)
        {
            TimeText.color = new Color(1, 0, 0);
        }
        else
        {
            TimeText.color = new Color(1, 0.78f, 0);
        }
        // Use ToString with "D2" format specifier for double digits
        TimeText.text = $"{minute:D2} : {second:D2}";
    }

    #endregion
}
