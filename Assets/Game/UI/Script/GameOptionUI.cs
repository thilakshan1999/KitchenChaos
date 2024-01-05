using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionUI : MonoBehaviour
{
    #region VARIABLE
    private Action endAction;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;

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
        Hide();
        setInitalMusicVolume();
        setInitalSoundVolume();
        UpdateKeyVisuals();
        AddBtnListener();
    }

    #endregion

    #region BTN FUNCTION
    public void OnClickBack()
    {
        Hide();
        endAction?.Invoke();
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

    internal void OpenOptionMenu(Action endAction)
    {
        this.endAction = endAction;
        Show();
    }
    #endregion

    #region MUSIC
    private void setInitalMusicVolume()
    {
        musicSlider.value = MusicManager.Instance.GetMusicVolume()*10f;
    }

    public void UpdateMusicVolume()
    {
        float volume = musicSlider.value / 10f;
        MusicManager.Instance.SetMusicVolume(volume);
    }
    #endregion

    #region SOUND
    private void setInitalSoundVolume()
    {
        soundSlider.value = SoundManager.Instance.GetSoundVolume() * 10f;
    }

    public void UpdateSoundVolume()
    {
        float volume = soundSlider.value / 10f;
        SoundManager.Instance.SetSoundVolume(volume);
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

    private void RebindBinding(GameInput.Binding binding)
    {
        GameInput.Instance.RebindBinding(binding, () => {
            UpdateKeyVisuals();
        });
    }

    private void AddBtnListener()
    {
        moveUp.onClick.AddListener(() =>
        {
            moveUp.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_";
            RebindBinding(GameInput.Binding.Move_Up);
        });
        moveDown.onClick.AddListener(() =>
        {
            moveDown.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_";
            RebindBinding(GameInput.Binding.Move_Down);
        });
        moveLeft.onClick.AddListener(() =>
        {
            moveLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_";
            RebindBinding(GameInput.Binding.Move_Left);
        });
        moveRight.onClick.AddListener(() =>
        {
            moveRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_";
            RebindBinding(GameInput.Binding.Move_Right);
        });
        interact.onClick.AddListener(() =>
        {
            interact.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_";
            RebindBinding(GameInput.Binding.Interact);
        });
        altInteract.onClick.AddListener(() =>
        {
            altInteract.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_";
            RebindBinding(GameInput.Binding.Alternat_Interact);
        });
        pause.onClick.AddListener(() =>
        {
            pause.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "_";
            RebindBinding(GameInput.Binding.Pause);
        });
    }
    #endregion
}
