using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharaterSelectPlayer : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyText;
    [SerializeField] private Transform playerParent;
    [SerializeField] private Button kickButton;

    private PlayerData playerdata;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        kickButton.onClick.AddListener(() => {
            OnClickCancel();
        });
    }

    private void Start()
    {
        KitchenNetworkMultiplayer.Instance.OnPlayerNetworkDataListChanged += KitchenNetworkMultiplayer_OnPlayerJoint;
        CharacterSelectReady.Instance.OnPlayerReady += CharacterSelectReady_OnPlayerReady;
        UpdatePlayer(); 
        HideReadyText();

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && playerIndex != 0);
    }

    private void OnDestroy()
    {
        KitchenNetworkMultiplayer.Instance.OnPlayerNetworkDataListChanged -= KitchenNetworkMultiplayer_OnPlayerJoint;
        CharacterSelectReady.Instance.OnPlayerReady -= CharacterSelectReady_OnPlayerReady;
    }
    #endregion

    #region CHARACTER FUNCTION
    private void KitchenNetworkMultiplayer_OnPlayerJoint()
    {
        UpdatePlayer();
    }

    private void CharacterSelectReady_OnPlayerReady()
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (KitchenNetworkMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            playerdata = KitchenNetworkMultiplayer.Instance.GetPlayerDataFromIndex(playerIndex);
            if (CharacterSelectReady.Instance.IsPlayerReady(playerdata.clientId))
            {
                ShowReadyText();
            }
            else
            {
                HideReadyText();
            }

            Show();
            SetPlayerCharacter(GetPlayerCharacterId(playerdata.playerCharacter));
        }
        else
        {
            Hide();
        }
    }

    private void SetPlayerCharacter(int playerCharacterId)
    {
        for(int i = 0;i < playerParent.childCount; i++)
        {
            if (i == playerCharacterId)
            {
                playerParent.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                playerParent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private int GetPlayerCharacterId(KitchenNetworkMultiplayer.PlayerCharacter playerCharacter)
    {
        switch (playerCharacter)
        {
            case KitchenNetworkMultiplayer.PlayerCharacter.SOPHIE:
                return 0;
            case KitchenNetworkMultiplayer.PlayerCharacter.BRAIN:
                return 1;
            case KitchenNetworkMultiplayer.PlayerCharacter.BYRCE:
                return 2;
            case KitchenNetworkMultiplayer.PlayerCharacter.KATE:
                return 3;
            case KitchenNetworkMultiplayer.PlayerCharacter.MEGAN:
                return 4;
            case KitchenNetworkMultiplayer.PlayerCharacter.REMY:
                return 5;
            default:
                return 0;
        }
    }

    #endregion

    #region UI
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ShowReadyText()
    {
        readyText.SetActive(true);
    }

    private void HideReadyText()
    {
        readyText.SetActive(false);
    }
    #endregion

    #region BUTTON
    public void OnClickCancel()
    {
        KitchenNetworkMultiplayer.Instance.kickPlayer(playerdata.clientId);
    }
    #endregion
}
