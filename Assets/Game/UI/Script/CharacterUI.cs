using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private Transform selected;
    [SerializeField] private KitchenNetworkMultiplayer.PlayerCharacter playerCharacter;
    #endregion

    #region UNITY CALLBACK
    private void Start()
    {
        HideSelected();
        KitchenNetworkMultiplayer.Instance.OnPlayerNetworkDataListChanged += KitchenNetworkMultiplayer_OnPlayerJoint;
    }

    private void OnDestroy()
    {
        KitchenNetworkMultiplayer.Instance.OnPlayerNetworkDataListChanged -= KitchenNetworkMultiplayer_OnPlayerJoint;
    }
    #endregion

    #region FUNCTION
    public void OnClickCharacterBtn()
    {
        KitchenNetworkMultiplayer.Instance.UpdateLocalPlayerCharacter(playerCharacter);
        ShowSelected();
    }

    private void UpdateCharacterbuttonSelection(KitchenNetworkMultiplayer.PlayerCharacter playerCharacter)
    {
        if(this.playerCharacter == playerCharacter)
        {
            ShowSelected();
        }
        else
        {
            HideSelected();
        }
    }

    private void ShowSelected()
    {
        selected.gameObject.SetActive(true);
    }

    private void HideSelected()
    {
        selected.gameObject.SetActive(false);
    }

    private void UpdateCharacterbuttonInteractivity(List<KitchenNetworkMultiplayer.PlayerCharacter> playerCharacterList)
    {
        foreach(KitchenNetworkMultiplayer.PlayerCharacter playerCharacter in playerCharacterList)
        {
            if(this.playerCharacter == playerCharacter)
            {
                SetNotInteractive();
                return;
            }
        }
        SetInteractive();
    }

    private void SetInteractive()
    {
        GetComponent<Button>().interactable = true;
    }

    private void SetNotInteractive()
    {
        GetComponent<Button>().interactable = false;
    }

    private void KitchenNetworkMultiplayer_OnPlayerJoint()
    {
        UpdateCharacterbuttonSelection(KitchenNetworkMultiplayer.Instance.GetLocalPlayerCharacter());
        UpdateCharacterbuttonInteractivity(KitchenNetworkMultiplayer.Instance.GetActivePlayerCharacterList());
    }
    #endregion
}
