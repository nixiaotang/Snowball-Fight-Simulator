using System;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerAudio : MonoBehaviour
{
    public Sound[] sounds;
    // private static PlayerAudio instance;
    private Animator anime;
    
    private void Awake()
    {
        // if (instance == null) { instance = this; }
        // else
        // {
        //     Destroy(gameObject);
        //     return;
        // }

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    private void Start()
    {
        anime = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if (anime.GetCurrentAnimatorStateInfo(0).IsName("Idle / Walk.Walking") && !isPlaying("Walking"))
        {
            Play("Walking");
        }
        else if (!anime.GetCurrentAnimatorStateInfo(0).IsName("Idle / Walk.Walking") && isPlaying("Walking"))
        {
            Stop("Walking");
        }

        if (anime.GetCurrentAnimatorStateInfo(0).IsName("Running.Running") && !isPlaying("Running"))
        {
            Play("Running");
        }
        else if (!anime.GetCurrentAnimatorStateInfo(0).IsName("Running.Running") && isPlaying("Running"))
        {
            Stop("Running");
        }
    }
    
    
    public void Play(string clipName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clipName);
        s.source.Play();
    }
    
    public void Stop(string clipName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clipName);
        s.source.Stop();
    }

    public bool isPlaying(string clipName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == clipName);
        return s.source.isPlaying;
    }
}
