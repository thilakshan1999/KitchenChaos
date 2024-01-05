using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryObjectUI : MonoBehaviour
{
    #region UNITY CALLBACKS
    private void Awake()
    {
        ResetDeliveryObjectUI();
    }
    #endregion

    #region CONTAINER
    internal void UpdateDeliverableObjectUI(List<RecipieScriptables> deliverableRecipeList)
    {
        Transform container;
        RecipieScriptables deliverableRecipe;

        InstanciateContainerUI(deliverableRecipeList);
        ResetDeliveryObjectUI();

        for(int i = 0; i < deliverableRecipeList.Count; i++)
        {
            deliverableRecipe = deliverableRecipeList[i];
            container = transform.GetChild(i);

            container.GetChild(0).GetComponent<TextMeshProUGUI>().text = deliverableRecipe.foodName;
            UpdateIcontUI(container,deliverableRecipe);

            container.gameObject.SetActive(true);
        }
    }

    private void ResetDeliveryObjectUI()
    {
        Transform container;

        for (int i = 0; i < transform.childCount; i++)
        {
            container = transform.GetChild(i);
            container.gameObject.SetActive(false);
        }
    }

    private void InstanciateContainerUI(List<RecipieScriptables> deliverRecipeList)
    {
        int recipieCount = deliverRecipeList.Count;
        int childUICount = transform.childCount;
        if (recipieCount > childUICount)
        {
            for (int i = 0; i < recipieCount - childUICount; i++)
            {
                Instantiate(transform.GetChild(0), transform);
            }
        }
    }
    #endregion

    #region ICON
    private void UpdateIcontUI(Transform container, RecipieScriptables deliverRecipe)
    {
        Transform iconListUI = container.GetChild(1);

        Transform icon;
        KitchenObjectScriptables kitchenObjectSO;

        InstanciateIcontUI(deliverRecipe,iconListUI);
        ResetIcontUI(iconListUI);

        for (int i = 0; i < deliverRecipe.ingredientsList.Count; i++)
        {
            kitchenObjectSO = deliverRecipe.ingredientsList[i];

            icon = iconListUI.GetChild(i);
            icon.GetComponent<Image>().sprite = kitchenObjectSO.kitchenObjectSprite;

            icon.gameObject.SetActive(true);
        }
    }

    private void ResetIcontUI(Transform iconListUI)
    {
        Transform icon;
        for (int i = 0; i < iconListUI.childCount; i++)
        {
            icon = iconListUI.GetChild(i);
            icon.gameObject.SetActive(false);
        }
    }

    private void InstanciateIcontUI(RecipieScriptables deliverRecipe,Transform iconListUI)
    {
        int recipieCount = deliverRecipe.ingredientsList.Count;
        int iconUICount = iconListUI.childCount;
        if (recipieCount > iconUICount)
        {
            for (int i = 0; i < recipieCount - iconUICount; i++)
            {
                Instantiate(iconListUI.GetChild(0), iconListUI);
            }
        }
    }

    #endregion
}
