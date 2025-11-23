using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;

public enum AudioTypes
{
    Default,
    Music,
    Ambient,
    UI,
    Pomodoro,
    Effects,
}

[Serializable]
public struct Audio
{
    public EventReference soundEvent;

    Audio (EventReference soundEvent)
    {
        this.soundEvent = soundEvent;
    }
}

[Serializable]
public struct AudioArray
{
    public List<Audio> possibleAudios;

    AudioArray(List<Audio> possibleAudios)
    {
        this.possibleAudios = possibleAudios;
    }

    public Audio GetRandom()
    {
        return Help.RandomElementInArray(possibleAudios);
    }
}

public class AudioHub : MonoBehaviour
{
    [SerializeField] private string masterBusPath;
    [SerializeField] private string musicBusPath;
    [SerializeField] private string uiBusPath;
    [SerializeField] private string effectsBusPath;
    
    private StudioEventEmitter musicPlayer;

    public void PlayClip(Audio clip, Vector3 Location = default)
    {
        if (!clip.soundEvent.IsNull)
        {
            EventInstance instance = RuntimeManager.CreateInstance(clip.soundEvent.Guid);
            if (Location != default)
            {
                instance.set3DAttributes(RuntimeUtils.To3DAttributes(Location));
            }
            instance.start();
            instance.release();
        }
    }

    public StudioEventEmitter SetupLoopingClip(Audio clip, Vector3 Location = default)
    {
        if (!clip.soundEvent.IsNull)
        {
            GameObject gO = new GameObject();
            gO.transform.parent = transform;

            StudioEventEmitter newAudioSource = gO.AddComponent<StudioEventEmitter>();
            AudioPlayer audioPlayerComponent = gO.AddComponent<AudioPlayer>();
            audioPlayerComponent.Initialize(newAudioSource);
            newAudioSource.EventReference = clip.soundEvent;
            newAudioSource.StopEvent = EmitterGameEvent.ObjectDestroy;
            if (Location != default)
            {
                gO.transform.position = Location;
            }
            newAudioSource.Stop();

            return newAudioSource;
        }

        return null;
    }

    public void DestroyLoopingClip(StudioEventEmitter LoopingClip)
    {
        if (LoopingClip == null)
        {
            return;
        }
        
        LoopingClip.gameObject.GetComponent<AudioPlayer>().DestroyPlayer();
    }

    public void PlayLoopingClip(StudioEventEmitter source)
    {
        if (source != null)
        {
            if (!source.IsPlaying() && !source.EventReference.IsNull)
            {
                source.Play();
            }
        }
    }

    public void StopLoopingClip(StudioEventEmitter source)
    {
        if (source != null)
        {
            source.Stop();
        }
    }

    public void SetMusic(Audio clip)
    {
        if (clip.soundEvent.IsNull)
        {
            DestroyLoopingClip(musicPlayer);
            Debug.Log("MUSIC START NULL");
            return;
        }
        
        if (musicPlayer != null && !musicPlayer.EventReference.Equals(clip.soundEvent))
        {
            SetMusic(new Audio());
        }
        
        musicPlayer = SetupLoopingClip(clip);
                
        if (!musicPlayer.IsPlaying())
        {
            Debug.Log($"MUSIC START {clip.ToString()}");
            musicPlayer.Play();
        }
    }

    public StudioEventEmitter GetMusicPlayer()
    {
        return musicPlayer;
    }

    /*private void OnVolumeChanged(AudioTypes audioType)
    {
        RuntimeManager.GetBus(masterBusPath).setVolume(GameWizardSaveDataI.Get().masterVolumeScalar);
        RuntimeManager.GetBus(musicBusPath).setVolume(GameWizardSaveDataI.Get().musicVolumeScalar);
        RuntimeManager.GetBus(ambienceBusPath).setVolume(GameWizardSaveDataI.Get().ambientVolumeScalar);
        RuntimeManager.GetBus(pomodoroBusPath).setVolume(GameWizardSaveDataI.Get().pomodoroVolumeScalar);
        RuntimeManager.GetBus(uiBusPath).setVolume(GameWizardSaveDataI.Get().UIVolumeScalar);
        RuntimeManager.GetBus(effectsBusPath).setVolume(GameWizardSaveDataI.Get().effectsVolumeScalar);
    }*/
}