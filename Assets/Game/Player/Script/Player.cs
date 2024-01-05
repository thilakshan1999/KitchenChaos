using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour ,IKitchenObjectCarrier
{
    #region VARIABLE
    [Header("SpawnPoestion")]
    [SerializeField] private List<Vector3> spawnPositionList;

    [Header("Mask")]
    [SerializeField] private LayerMask containerMask;

    [Header("PROPERTIES")]
    private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float accelaration;
    private bool isWalking;
    

    [Header("SELECT_COUNTER")]
    IKitchenCounter currentKitchenCounter = null;
    float rayHitRange = 1f;
    RaycastHit raycastHit ;

    [Header("KITCHEN OBJECT")]
    [SerializeField] private Transform kitchenObjectParent;
    private KitchenObject kitchenObject;

    internal event Action OnPickUpAndDropObject;

    #endregion

    #region UNIY CALLBACKS

    private void Start()
    {
        GameInput.Instance.OnInteractionAction += Player_OnInteractionAction;
        GameInput.Instance.OnAlternativeInteractionAction += Player_OnAlternativeInteractionAction;
    }

    private void Update()
    {
        if (!IsOwner) return;
        UpdateMovement();
        SelectCounter();
    }

    #endregion

    #region PLAYER MOVEMENT

    private void UpdateMovement()
    {
        if (GameManager.Instance.gameState.Value != GameManager.GameState.GAME_PLAYING) return;
        Vector3 movementVector = UpdateInputVector();

        SetSpeed(movementVector);
        UpdatePoestion(movementVector);
        UpdateRotation();
    }

    private Vector3 UpdateInputVector()
    {
        Vector2 inputVector = GameInput.Instance.GetNormalizedInputVector();
        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);

        Vector3 lowPoint = transform.localPosition;
        Vector3 highPoint = transform.localPosition + new Vector3(0, 1.8f, 0);
        float capsuleRadius = 0.3f;

        Vector3 movmentVectorX = new Vector3(movementVector.x,0,0);
        Vector3 movmentVectorZ = new Vector3(0, 0, movementVector.z);

        if (Physics.CapsuleCast(lowPoint, highPoint, capsuleRadius, movmentVectorX,0.1f))
        {
            
            if(Physics.CapsuleCast(lowPoint, highPoint, capsuleRadius, movmentVectorZ, 0.1f))
            {
                movementVector = Vector3.zero;
            }
            else
            {
                movementVector = movmentVectorZ;
            }
        }
        else if (Physics.CapsuleCast(lowPoint, highPoint, capsuleRadius, movmentVectorZ, 0.1f))
        {
            movementVector = movmentVectorX;
        }

        return movementVector;
    }

    private void UpdatePoestion(Vector3 movementVector)
    {
        transform.localPosition += movementVector * Time.deltaTime * speed;

    }

    private void UpdateRotation()
    {
        Vector2 inputVector = GameInput.Instance.GetNormalizedInputVector();
        Vector3 movementVector = new Vector3(inputVector.x, 0, inputVector.y);

        transform.forward = Vector3.Slerp
            (transform.forward, movementVector, rotationSpeed * Time.deltaTime);
    }

    #endregion

    #region PLAYER PROPERTIES

    private void SetSpeed(Vector3 movementVector)
    {
        if (movementVector.magnitude > 0.1f && speed <= maxSpeed)
        {
            if (!isWalking)
            {
                SoundManager.Instance.PlayWalkingSound();
                isWalking = true;
            }
            
            speed += movementVector.magnitude * accelaration * Time.deltaTime;
        }
        else if(speed >= 0)
        {
            if (isWalking)
            {
                SoundManager.Instance.StopWalkingSound();
                isWalking = false;
            }
            
            speed -=  accelaration * Time.deltaTime;
        }
        speed = Mathf.Clamp(speed, 0f, maxSpeed);
    }

    internal float GetPlayerSpeed()
    {
        return speed;
    }

    internal float GetMaxSpeed()
    {
        return maxSpeed;
    }

    #endregion

    #region INTERACTION
    private void SelectCounter()
    {
        if (Physics.Raycast(transform.position, transform.forward,out raycastHit, rayHitRange,containerMask))
        {
            if (raycastHit.collider.gameObject.TryGetComponent(out IKitchenCounter kitchenCounter))
            {
                if (currentKitchenCounter != kitchenCounter)
                {
                    if (currentKitchenCounter != null)
                    {
                        currentKitchenCounter.CounterDeselected();
                        currentKitchenCounter = null;
                    }
                    currentKitchenCounter = kitchenCounter;
                    currentKitchenCounter.CounterSelected();
                }
            }
        }
        else
        {
            if (currentKitchenCounter != null)
            {
                currentKitchenCounter.CounterDeselected();
                currentKitchenCounter = null;
            }
        }
    }

    private void Player_OnInteractionAction(object sender, System.EventArgs e)
    {
        if (currentKitchenCounter != null && GameManager.Instance.gameState.Value == GameManager.GameState.GAME_PLAYING)
        {
            currentKitchenCounter.Interact(this);
        }
    }

    private void Player_OnAlternativeInteractionAction(object sender, EventArgs e)
    {
        if (currentKitchenCounter != null && GameManager.Instance.gameState.Value == GameManager.GameState.GAME_PLAYING)
        {
            currentKitchenCounter.AlternativeInteract(this);
        }
    }
    #endregion

    #region KITCHEN OBJECT

    public bool CheckKitchenObjectIsEmpty()
    {
        return kitchenObject == null;
    }

    public void SetKitchenObject( KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
        OnPickUpAndDropObject?.Invoke();
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void RemoveKitchenObject()
    {
        kitchenObject = null;
        OnPickUpAndDropObject?.Invoke();
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }

    public Transform GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void AttachKitchenObjectWithCarrier(KitchenObject kitchenObject)
    {
        kitchenObject.TranformKitchenObjectToCarrier(this);
    }

    #endregion

    #region NETCODE
    public override void OnNetworkSpawn()
    {
        transform.position = spawnPositionList[KitchenNetworkMultiplayer.Instance.GetPlayerId(OwnerClientId)];

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if (clientId == OwnerClientId)
        {
            if (!CheckKitchenObjectIsEmpty())
            {
                GetKitchenObject().TranformBackToStorage();
            }
        }
    }
    #endregion
}

