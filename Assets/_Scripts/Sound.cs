using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 4f)]
    public float volume = 1f;
    [Range(0.1f, 3f)][HideInInspector]
    public float pitch = 1f;

    [HideInInspector]
    public AudioSource source;
}
