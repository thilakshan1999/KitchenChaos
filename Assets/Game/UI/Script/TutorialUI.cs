using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    #region VARIABLE
    [SerializeField] private Button moveUp;
    [SerializeField] private Button moveDown;
    [SerializeField] private Button moveLeft;
    [SerializeField] private Button moveRight;
    [SerializeField] private Button interact;
    [SerializeField] private Button altInteract;
    [SerializeField] private Button pause;
    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        Show();
        UpdateKeyVisuals();
        GameManager.Instance.OnLocalPlayerInteract += GameManger_OnLocalPlayerInteract;
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

    private void GameManger_OnLocalPlayerInteract()
    {
        Hide();
    }
    #endregion


    #region CONTROL
    private void UpdateKeyVisuals()
    {
        moveUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
        moveDown.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
        moveLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
        moveRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
        interact.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
        altInteract.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameInput.Instance.GetBindingText(GameInput.Binding.Alternat_Interact);
        pause.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
    }
    #endregion
}
