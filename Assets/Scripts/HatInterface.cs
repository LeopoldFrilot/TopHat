using System;
using ScriptableObjects;
using UnityEngine;

public class HatInterface : MonoBehaviour
{
    private Fighter fighter;
    private PlayerHat playerHat;
    private HatStats hatStats;
    private HatMovementAbility spawnedMovementAbility;
    private HatMainAbility spawnedMainAbility;

    private void Awake()
    {
        fighter = GetComponent<Fighter>();
    }

    public Action<PlayerHat> OnHatSet;
    public void SetHat(PlayerHat playerHat)
    {
        if (playerHat != this.playerHat)
        {
            this.playerHat = playerHat;
            InstallHatStats(playerHat ? playerHat.GetHatStats() : null);
            OnHatSet?.Invoke(playerHat);
        }
    }

    public void InstallHatStats(HatStats hatStats)
    {
        if (hatStats == this.hatStats)
        {
            return;
        }
        
        DestroyImmediate(spawnedMovementAbility);
        DestroyImmediate(spawnedMainAbility);

        if (hatStats == null)
        {
            return;
        }
        
        spawnedMovementAbility = Instantiate(hatStats.movementAbilityPrefab, transform);
        spawnedMainAbility = Instantiate(hatStats.mainAbilityPrefab, transform);
    }

    public void OnMovementActionStarted()
    {
        spawnedMovementAbility.TryActivate();
    }

    public void OnMovementActionCancelled()
    {
        spawnedMovementAbility.TryCancel();
    }

    public void OnMainActionCancelled()
    {
        spawnedMainAbility.TryActivate();
    }

    public void OnMainActionStarted()
    {
        spawnedMainAbility.TryCancel();
    }

    public HatMainAbility GetMainAbility()
    {
        return spawnedMainAbility;
    }

    public HatMovementAbility GetMovementAbility()
    {
        return spawnedMovementAbility;
    }
}