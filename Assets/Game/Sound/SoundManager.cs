using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region VARIABLE

    public static SoundManager Instance { get; private set; }
    
    private const string PLAYER_PREFS_SOUND_VOLUME = "Sound Volume";
    private float volume;

    public event Action OnSoundVolumeChanged;

    [SerializeField] private AudioSource walk;
    [SerializeField] private AudioSource baseCounter;
    [SerializeField] private AudioClipSO audioClipSO;

    #endregion

    #region UNITY CALLBACKS

    private void Awake()
    {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_VOLUME, 0.5f);
    }

    #endregion

    #region FUNCTION

    internal void PlayWalkingSound()
    {
        walk.volume = volume;
        walk.Play();
    }

    internal void StopWalkingSound()
    {
        walk.Stop();
    }

    internal void PlayDeliverySucessSound()
    {
        PlaySound(audioClipSO.deleiverySucess, Vector3.one);
    }

    internal void PlayDeliveryFailSound()
    {
        PlaySound(audioClipSO.deliveryFail, Vector3.one);
    }

    internal void PlayTrashSound()
    {
        PlaySound(audioClipSO.trash, Vector3.one);
    }

    internal void PlayChopSound()
    {
        PlaySound(audioClipSO.chop, Vector3.one);
    }

    internal void PlayPickUpSound(Vector3 poestion)
    {
        PlaySound(audioClipSO.objectPickup, poestion);
    }

    internal void PlayCountDownSound()
    {
        PlaySound(audioClipSO.warning, Vector3.one);
    }

    internal void PlayStoveWarningSound()
    {
        PlaySound(audioClipSO.warning, Vector3.one);
    }
    private void PlaySound(AudioClip audioClip,Vector3 poestion)
    {
        // AudioSource.PlayClipAtPoint(audioClip, poestion,volume);
        baseCounter.Stop();
        baseCounter.volume = volume;
        baseCounter.clip = audioClip;
        baseCounter.Play();
    }
    #endregion

    #region GAME SETUP
    internal void SetSoundVolume(float volume)
    {
        this.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_VOLUME, volume);
        PlayerPrefs.Save();
        OnSoundVolumeChanged?.Invoke(); 
    }

    internal float GetSoundVolume()
    {
        return volume;
    }


    #endregion
}
