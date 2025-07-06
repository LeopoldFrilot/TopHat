using System;
using UnityEngine;

public static class EventHub
{
    public static event Action<Player, PlayerFist, PlayerFistState> OnFistConnected;

    public static void TriggerFistConnected(Player playerHit, PlayerFist fistLanded, PlayerFistState fistState)
    {
        OnFistConnected?.Invoke(playerHit, fistLanded, fistState);
    }
    
    public static event Action<Player> OnPlayerEarnedPoints;
    public static void TriggerPlayerEarnedPoints(Player player)
    {
        OnPlayerEarnedPoints?.Invoke(player);
    }

    public static event Action<Player> OnPlayerGrappled;
    public static void TriggerPlayerGrappled(Player player)
    {
        OnPlayerGrappled?.Invoke(player);
    }

    public static event Action<AudioClip, float> OnPlaySoundRequested;
    public static void TriggerPlaySoundRequested(AudioClip clip, float volume = 1f)
    {
        OnPlaySoundRequested?.Invoke(clip, volume);
    }

    public static event Action<Player> OnTurnEnded;
    public static void TriggerTurnEnded(Player player)
    {
        OnTurnEnded?.Invoke(player);
    }

    public static event Action OnGameStarted;

    public static void TriggerGameStarted()
    {
        OnGameStarted?.Invoke();
    }
}
