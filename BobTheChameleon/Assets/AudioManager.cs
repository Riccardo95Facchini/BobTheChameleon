using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    // Use this for initialization
    void Awake()
    {
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    // Update is called once per frame
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        if (PauseMenu.GameIsPaused) {
            s.source.pitch *= .5f;
        }


        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.Stop();
    }

    public bool IsPlaying(string name)
    {

        return Array.Find(sounds, sound => sound.name == name).source.isPlaying;

    }

    public void ChangePitch(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if(s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }

        s.source.pitch = pitch;
    }

    void Start()
    {
        Play("Background music");
    }
}
