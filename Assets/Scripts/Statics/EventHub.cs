using System;
using UnityEngine;

public static class EventHub
{
    public static event Action<AudioClip, float> OnPlaySoundRequested;
    public static void TriggerPlaySoundRequested(AudioClip clip, float volume = 1f)
    {
        OnPlaySoundRequested?.Invoke(clip, volume);
    }

    public static event Action<Fighter> OnTurnEnded;
    public static void TriggerTurnEnded(Fighter fighter)
    {
        OnTurnEnded?.Invoke(fighter);
    }

    public static event Action OnGameStarted;

    public static void TriggerGameStarted()
    {
        OnGameStarted?.Invoke();
    }
}
