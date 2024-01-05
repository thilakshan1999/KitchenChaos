using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    #region VARIABLE

    private Animator animator;
    [SerializeField] private Player player;

    private const string SPEED = "MoveSpeed";
    private const string IS_OBJECT_IN_HAND = "IsObjectInHand";
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        animator = GetComponent<Animator>();
        player.OnPickUpAndDropObject += Player_OnPickUpAndDropObject;
    }

    

    private void Update()
    {
        if (!IsOwner) return;
        SetSpeed(player.GetPlayerSpeed(),player.GetMaxSpeed());
    }
    #endregion

    #region MOVEMENT
    private void SetSpeed(float playerSpeed,float maxSpeed)
    {
        float speed;

        speed = playerSpeed / maxSpeed;

        animator.SetFloat(SPEED, speed);
    }

    private void SetIsObjectInHand(bool hasKitchenObject)
    {
        animator.SetBool(IS_OBJECT_IN_HAND, hasKitchenObject);
    }

    private void Player_OnPickUpAndDropObject()
    {
        SetIsObjectInHand(!player.CheckKitchenObjectIsEmpty());
    }
    #endregion
}
