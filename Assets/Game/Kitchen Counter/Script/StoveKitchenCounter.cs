using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StoveKitchenCounter : BaseKitchenCounter
{
    #region VARIABLE
    private enum State 
    {
        IDLE,
        COOKING,
        COOKED,
        BURNED
    }

    private State currentState;

    [SerializeField] private GameObject select;

    [Header("KITCHEN OBJECT")]
    [SerializeField] private Transform kitchenStoreParent;
    [SerializeField] private Transform kitchenObjectParent;
    [SerializeField] private List<CookingObjectScriptables> cookinObjectsSOList;
    private CookingObjectScriptables cookingObjectSO;
    private float timer;

    [Header("Animation")]
    [SerializeField] Transform stoveOnVisual;
    [SerializeField] Transform sizzlineParticles;

    [Header("UI")]
    [SerializeField] private GameObject progressBar;

    [Header("Sound")]
    private float warinigTimer;
    private float maxWarinigTimer = 0.5f;
    private AudioSource audioSource;
    private bool isPaused;
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetKitchenObjectParent(kitchenObjectParent);

        currentState = State.IDLE;

        stoveOnVisual.gameObject.SetActive(false);
        sizzlineParticles.gameObject.SetActive(false);

        DeactiveProgressBar();

        audioSource.Stop();
        audioSource.volume = SoundManager.Instance.GetSoundVolume();
        SoundManager.Instance.OnSoundVolumeChanged += SoundManager_OnSoundVolumeChanged;
        GameManager.Instance.OnToggleSound += GameManager_OnToggleSound;
    }

    private void Update()
    {
        if (!IsServer) return;
        TimerFunction();
        WarningTimer();
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
            if (CheckKitchenObjectIsEmpty())
            {
                //player and counter in null
            }
            else
            {
                //conter has object, player is null
                player.AttachKitchenObjectWithCarrier(GetKitchenObject());
                UpdateRemoveObjectToServerRpc();
            }
        }
        else
        {
            if (CheckKitchenObjectIsEmpty())
            {
                //player has object , couter is null
                if (!CheckCookingObjectSOIsEmpty(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    //player has cookableobject
                    AttachKitchenObjectWithCarrier(player.GetKitchenObject());
                    player.RemoveKitchenObject();
                }
                else
                {
                    //player has not have cookable object;
                }
            }

            else
            {
                //player and counter both have object
                if (player.GetKitchenObject() is DeliveryKitchenObject)
                {
                    DeliveryKitchenObject deliveryKitchenObject = player.GetKitchenObject() as DeliveryKitchenObject;
                    if (deliveryKitchenObject.CheckKitchenObjectIsDeleliverable(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        if (deliveryKitchenObject.TryToAddKitchenObjectToDeliveryObjectList(GetKitchenObject()))
                        {
                            GetKitchenObject().TranformBackToStorage();
                            UpdateRemoveObjectToServerRpc();
                        }

                    }
                }
            }
        }
    }

    public override void AlternativeInteract(Player player)
    {
        if (!CheckKitchenObjectIsEmpty())
        {
            if (cookingObjectSO.uncookedKitchenObjectSO == GetKitchenObject().GetKitchenObjectSO())
            {
                UpdateEffectToServerRpc();
            }
            else
            {
                //Already cooked;
            }
        }
        else
        {
            //Kitchen object is empty
        }
    }
    #endregion

    #region KITCHEN OBJECT

    private bool CheckCookingObjectSOIsEmpty(KitchenObjectScriptables kitchenObjectSO)
    {
        foreach (CookingObjectScriptables cookinObjectSO in cookinObjectsSOList)
        {
            if (cookinObjectSO.uncookedKitchenObjectSO == kitchenObjectSO)
            {
                this.cookingObjectSO = cookinObjectSO;
            }
        }
        return cookingObjectSO == null;
    }

    private void GetCookedKitchenObject()
    {
        KitchenObjectScriptables cookedKitchenObjectSo = cookingObjectSO.cookedKitchenObjectSO;
        KitchenStorage kitchenStorage = GetKitchenStorage(cookedKitchenObjectSo, kitchenStoreParent);
        kitchenStorage.GetKitchenObject(this);
    }

    private void GetBurnedKitchenObject()
    {
        KitchenObjectScriptables burnedKitchenObjectSo = cookingObjectSO.burnedKitchenObjectSO;
        KitchenStorage kitchenStorage = GetKitchenStorage(burnedKitchenObjectSo, kitchenStoreParent);
        kitchenStorage.GetKitchenObject(this);
    }

    #endregion

    #region TIMER
    private void TimerFunction()
    {
        switch (currentState)
        {
            case State.IDLE:
                break;
            case State.COOKING:
                if (CheckCookingObjectSOIsEmpty(GetKitchenObject().GetKitchenObjectSO())) return;
                if (timer >= cookingObjectSO.cookingTime)
                {
                    timer = 0;

                    currentState = State.COOKED;

                    GetKitchenObject().TranformBackToStorage();
                    RemoveKitchenObject();
                    GetCookedKitchenObject();

                    UpdateWarnigUIEffectToClientRpc();
                }
                else
                {
                    timer += Time.deltaTime;
                    UpdateProgressIncreseEffectToClientRpc(timer, cookingObjectSO.cookingTime);
                }
                break;
            case State.COOKED:
                if (timer >= cookingObjectSO.burningTime)
                {
                    timer = 0;

                    currentState = State.BURNED;

                    GetKitchenObject().TranformBackToStorage();
                    RemoveKitchenObject();
                    GetBurnedKitchenObject();
                }
                else
                {
                    timer += Time.deltaTime;
                    UpdateProgressIncreseEffectToClientRpc(timer, cookingObjectSO.burningTime);
                }
                break;
            case State.BURNED:
                UpdateBurnedEffectToClientRpc();
                break;
        }
    }

    private void WarningTimer()
    {
        if (currentState != State.COOKED) return;
        if (warinigTimer <= 0)
        {
            warinigTimer = maxWarinigTimer;
            UpdateWarnigSoundEffectToClientRpc();
        }
        else
        {
            warinigTimer -= Time.deltaTime;
        }

    }
    #endregion

    #region UI
    private void ActiveProgressBar()
    {
        if (!progressBar.activeSelf)
        {
            progressBar.SetActive(true);
            progressBar.transform.GetChild(3).gameObject.SetActive(false);
        }
    }

    private void DeactiveProgressBar()
    {
        if (progressBar.activeSelf)
        {
            progressBar.SetActive(false);
            progressBar.transform.GetChild(2).GetComponent<Image>().fillAmount = 0;
            ModifyProgressBar(false);
        }
    }

    private void IncreseProgress(float time,float maxTime)
    {
        progressBar.transform.GetChild(2).GetComponent<Image>().fillAmount = (float)time / maxTime;
    }

    private void ModifyProgressBar(bool isCooked)
    {
        if (isCooked)
        {
            progressBar.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 0, 0, 1);
            progressBar.transform.GetChild(3).gameObject.SetActive(true);
        }
        else
        {
            progressBar.transform.GetChild(2).GetComponent<Image>().color = new Color(1, 0.72f, 0, 1);
            progressBar.transform.GetChild(3).gameObject.SetActive(false);
        }
    }


    #endregion

    #region SOUND
    private void SoundManager_OnSoundVolumeChanged()
    {
        audioSource.volume = SoundManager.Instance.GetSoundVolume();
    }

    private void PauseSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPaused = true;
        }
    }

    private void ResumeSound()
    {
        if (isPaused)
        {
            isPaused = false;
            audioSource.Play();
        }
    }

    private void GameManager_OnToggleSound()
    {
        if (GameManager.Instance.GetIsGamePaused())
        {
            PauseSound();
        }
        else
        {
            ResumeSound();
        }
    }
    #endregion

    #region NETCODE
    [ServerRpc(RequireOwnership = false)]
    private void UpdateRemoveObjectToServerRpc()
    {
        timer = 0;
        warinigTimer = 0;
        currentState = State.IDLE;
        UpdateRemoveObjectToClientRpc();
    }

    [ClientRpc]
    private void UpdateRemoveObjectToClientRpc()
    {
        stoveOnVisual.gameObject.SetActive(false);
        sizzlineParticles.gameObject.SetActive(false);

        RemoveKitchenObject();
        cookingObjectSO = null;

        DeactiveProgressBar();
        audioSource.Stop();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateEffectToServerRpc()
    {
        timer = 0;
        currentState = State.COOKING;
        UpdateEffectToClientRpc();
    }

    [ClientRpc]
    private void UpdateEffectToClientRpc()
    {
        ActiveProgressBar();
        stoveOnVisual.gameObject.SetActive(true);
        sizzlineParticles.gameObject.SetActive(true);
        audioSource.Play();
    }

    [ClientRpc]
    private void UpdateWarnigSoundEffectToClientRpc()
    {
        SoundManager.Instance.PlayStoveWarningSound();
    }

    [ClientRpc]
    private void UpdateWarnigUIEffectToClientRpc()
    {
        ModifyProgressBar(true);
    }

    [ClientRpc]
    private void UpdateBurnedEffectToClientRpc()
    {
        DeactiveProgressBar();
        audioSource.Stop();
    }

    [ClientRpc]
    private void UpdateProgressIncreseEffectToClientRpc(float time, float maxTime)
    {
        IncreseProgress(time, maxTime);
    }

    #endregion
}
