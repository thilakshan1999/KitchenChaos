using Unity.Netcode;
using UnityEngine;

public class ContainerKitchenCounter : BaseKitchenCounter
{
    #region VARIABLE

    [SerializeField] private Animator animator;
    private const string OPEN_CLOSE = "OpenClose";

    [SerializeField] private SpriteRenderer objectSprite;

    [SerializeField] private GameObject select;

    [SerializeField] private KitchenObjectScriptables kitchenObjectSO;

    [SerializeField] private Transform kitchenStorageParent;
    private KitchenStorage kitchenStorage;

    #endregion

    #region UNITY CALLBACKS
    private void Start()
    {
        objectSprite.sprite = kitchenObjectSO.kitchenObjectSprite;
        kitchenStorage = GetKitchenStorage(kitchenObjectSO,kitchenStorageParent);
    }

    #endregion

    #region SELECTED

    public override void CounterSelected()
    {
        select.gameObject.SetActive(true);
    }

    public override void CounterDeselected()
    {
        select.gameObject.SetActive(false);
    }

    #endregion

    #region INTERACT

    public override void Interact(Player player)
    {
        if (player.CheckKitchenObjectIsEmpty())
        {
            UpdateContainerEffectToServerRpc();
            kitchenStorage.GetKitchenObject(player);
        }
    }
    #endregion

    #region NETCODE
    [ServerRpc(RequireOwnership =false)]
    private void UpdateContainerEffectToServerRpc()
    {
        UpdateContainerEffectToClientRpc();
    }

    [ClientRpc]
    private void UpdateContainerEffectToClientRpc()
    {
        animator.SetTrigger(OPEN_CLOSE);
        SoundManager.Instance.PlayPickUpSound(transform.position);
    }
    #endregion
}
