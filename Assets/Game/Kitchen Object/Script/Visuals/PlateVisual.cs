using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateVisual : MonoBehaviour
{
    #region VARIABLE

    [SerializeField] private List<VisualObject> visualObjectList;
    [SerializeField] private Transform plateIconUI;
    #endregion

    #region UNITY CALLBACK
    private void Awake()
    {
        DisableAllVisuals();
    }

    #endregion

    #region PLATE VISUAL FUNCTION

    internal void DisableAllVisuals()
    {
        foreach (VisualObject visualObject in visualObjectList)
        {
            visualObject.kitchenObjectVisual.gameObject.SetActive(false);
        }
        for (int i = 0; i < plateIconUI.childCount; i++)
        {
            plateIconUI.GetChild(i).gameObject.SetActive(false);
        }
    }

    internal void EnableVisual(KitchenObjectScriptables kitchenObjectSO)
    {
        Transform icon;

        foreach (VisualObject visualObject in visualObjectList)
        {
            if(visualObject.KitchenObjectSO == kitchenObjectSO)
            {
                visualObject.kitchenObjectVisual.gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < plateIconUI.childCount; i++)
        {
            icon = plateIconUI.GetChild(i);
            if (!icon.gameObject.activeSelf)
            {
                icon.GetChild(1).GetComponent<Image>().sprite =
                    kitchenObjectSO.kitchenObjectSprite;
                icon.gameObject.SetActive(true);
                return;
            }
        }

        icon = Instantiate(plateIconUI.GetChild(0), plateIconUI);
        icon.GetChild(1).GetComponent<Image>().sprite =
                    kitchenObjectSO.kitchenObjectSprite;
        icon.gameObject.SetActive(true);

    }

    #endregion


    [Serializable]
    private struct VisualObject 
    {
        public KitchenObjectScriptables KitchenObjectSO;
        public Transform kitchenObjectVisual;
    }
}
