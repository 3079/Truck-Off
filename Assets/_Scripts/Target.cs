using System.Collections;
using System.Collections.Generic;
using _Scripts;
using UnityEngine;
using UnityEngine.Audio;

public class Target : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 4;
    // [SerializeField] private SoundHolder _soundHolder;
    // private Sound[] hitSounds;
    [SerializeField] Sound[] hitSounds;

    public virtual void Awake()
    {
        // hitSounds = _soundHolder.Sounds;
        
        foreach(Sound s in hitSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = 1f;
            s.source.loop = false;
            //s.source.outputAudioMixerGroup = sfxMixer;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        Sound sound = hitSounds[UnityEngine.Random.Range(0, hitSounds.Length)];
        sound.source.Play();
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die ()
    {
        Destroy(gameObject);
    }
}
