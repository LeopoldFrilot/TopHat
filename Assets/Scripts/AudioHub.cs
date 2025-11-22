using System;
using UnityEngine;

public class AudioHub : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnPlaySoundRequested(AudioClip clip, float volume)
    {
        audioSource.PlayOneShot(clip, volume);
    }

    public AudioSource SetUpLoopingAudio(AudioClip clip, float volume)
    {
        if (!clip)
        {
            return null;
        }
        
        GameObject newObject = Instantiate(new GameObject(), transform);
        AudioSource newSource = newObject.AddComponent<AudioSource>();
        newSource.loop = true;
        newSource.playOnAwake = false;
        newSource.clip = clip;
        newSource.volume = volume;
        newSource.Stop();
        
        return newSource;
    }

    public void DestroyLoopingAudio(AudioSource source)
    {
        if (source)
        {
            Destroy(source.gameObject);
        }
    }
    
    private void OnEnable()
    {
        EventHub.OnPlaySoundRequested += OnPlaySoundRequested;
    }

    private void OnDisable()
    {
        EventHub.OnPlaySoundRequested -= OnPlaySoundRequested;
    }
}