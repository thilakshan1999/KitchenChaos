using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    #region VARIABLE

    public static GameInput Instance { get; private set; }

    private const string PLAYER_PREFS_BINDINGS = "InputBindings";
    private PlayerInputAction playerInputActions;

    public event EventHandler OnInteractionAction;
    public event EventHandler OnAlternativeInteractionAction;
    public event EventHandler OnPauseAction;

    public enum Binding
    {
        Move_Up,
        Move_Down,
        Move_Left,
        Move_Right,
        Interact,
        Alternat_Interact,
        Pause
    }
    #endregion

    #region UNITY CALLBACKS

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        playerInputActions = new PlayerInputAction();
        if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
        }
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interaction.performed += Interaction_performed;
        playerInputActions.Player.AlternateInteraction.performed += AlternateInteraction_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Disable();

        playerInputActions.Player.Interaction.performed -= Interaction_performed;
        playerInputActions.Player.AlternateInteraction.performed -= AlternateInteraction_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;

        playerInputActions.Dispose();
    }
    #endregion

    #region MOVEMENT

    internal Vector2 GetNormalizedInputVector()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    #endregion

    #region INTERACTION

    private void Interaction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractionAction?.Invoke(this, EventArgs.Empty);
    }

    private void AlternateInteraction_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAlternativeInteractionAction?.Invoke(this, EventArgs.Empty);
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.gameState.Value == GameManager.GameState.GAME_PLAYING)
        {
            OnPauseAction?.Invoke(this, EventArgs.Empty);
        }
    }
    #endregion

    #region BINDING
    internal string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            case Binding.Move_Up:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.Move_Down:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.Move_Left:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.Move_Right:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.Interact:
                return playerInputActions.Player.Interaction.bindings[0].ToDisplayString();
            case Binding.Alternat_Interact:
                return playerInputActions.Player.AlternateInteraction.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
            default:
                return playerInputActions.Player.Interaction.bindings[0].ToString();
        }
    }

    internal void RebindBinding(Binding binding, Action onActionRebound)
    {

        playerInputActions.Player.Disable();

        InputAction inputAction;
        int bindindIndex;

        switch (binding)
        {
            case Binding.Move_Up:
                inputAction = playerInputActions.Player.Move;
                bindindIndex = 1;
                break;
            case Binding.Move_Down:
                inputAction = playerInputActions.Player.Move;
                bindindIndex = 2;
                break;
            case Binding.Move_Left:
                inputAction = playerInputActions.Player.Move;
                bindindIndex = 3;
                break;
            case Binding.Move_Right:
                inputAction = playerInputActions.Player.Move;
                bindindIndex = 4;
                break;
            case Binding.Interact:
                inputAction = playerInputActions.Player.Interaction;
                bindindIndex = 0;
                break;
            case Binding.Alternat_Interact:
                inputAction = playerInputActions.Player.AlternateInteraction;
                bindindIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bindindIndex = 0;
                break;
            default:
                inputAction = playerInputActions.Player.Move;
                bindindIndex = 1;
                break;
        }
        inputAction.PerformInteractiveRebinding(bindindIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                playerInputActions.Player.Enable();
                onActionRebound();
                PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            })
            .Start();
    }
    #endregion

}
