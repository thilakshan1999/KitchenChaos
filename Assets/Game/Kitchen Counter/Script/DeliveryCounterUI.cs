using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryCounterUI : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private Image bacground;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Color sucessColor;
    [SerializeField] private Color failColor;
    [SerializeField] private Sprite sucessSprite;
    [SerializeField] private Sprite failSprite;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    #endregion

    #region FUNCTION
    internal void OnSucessfulDelivery()
    {
        bacground.color = sucessColor;
        icon.sprite = sucessSprite;
        text.text = "Deliver\nSucess";
        gameObject.SetActive(true);
        StartCoroutine(hideUI());
    }

    internal void OnFailDelivery()
    {
        bacground.color = failColor;
        icon.sprite = failSprite;
        text.text = "Deliver\nFail";
        gameObject.SetActive(true);
        StartCoroutine(hideUI());
    }

    private IEnumerator hideUI()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
    #endregion
}
