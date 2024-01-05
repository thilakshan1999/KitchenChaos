using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeliverObjectCountUI : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private TextMeshProUGUI countText;

    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        GameManager.Instance.OnDeliverCountChanged += GameManager_OnDeliverCountChanged;
    }
    #endregion

    #region SUBSCRIBED FUNCTION
    private void GameManager_OnDeliverCountChanged()
    {
        countText.text = GameManager.Instance.GetDeliverdRecipieCount().ToString();
    }
    #endregion
}
