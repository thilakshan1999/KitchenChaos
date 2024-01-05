using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio Clip Object", menuName = "Kitchen PLA/Audio Clip Object")]
public class AudioClipSO : ScriptableObject
{
    public AudioClip chop;
    public AudioClip deliveryFail;
    public AudioClip deleiverySucess;
    public AudioClip footstep;
    public AudioClip objectPickup;
    public AudioClip stoveSizzle;
    public AudioClip trash;
    public AudioClip warning;
}
