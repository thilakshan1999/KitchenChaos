using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    #region VARIABLE
    public static MusicManager Instance { get; private set; }
    private AudioSource audioSource;
    private const string PLAYER_PREFS_MUSIC_VOLUME= "Music Volume";
    #endregion

    #region UNITY CALLBACKS
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        audioSource = transform.GetComponent<AudioSource>();
        audioSource.volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME,0.5f);
    }
    #endregion

    #region GAME SETUP
    internal void SetMusicVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    internal float GetMusicVolume()
    {
        return audioSource.volume;
    }
    #endregion
}

