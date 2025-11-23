using System;
using System.Collections.Generic;
using UnityEngine;

public class HatInterface : MonoBehaviour
{
    private Fighter fighter;
    private PlayerStatus playerStatus;
    private PlayerHat playerHat;
    private HatStats hatStats;
    private HatMovementAbility spawnedMovementAbility;
    private HatMainAbility spawnedMainAbility;

    private void Awake()
    {
        fighter = GetComponent<Fighter>();
        playerStatus = fighter.GetComponent<PlayerStatus>();
        playerStatus.OnStatusEffectsChanged += OnStatusEffectsChanged;
    }

    public Action<PlayerHat> OnHatSet;
    public void SetHat(PlayerHat playerHat)
    {
        if (playerHat != this.playerHat)
        {

            if (playerHat)
            {
                InstallHatStats(playerHat.GetHatStats());
            }
            else
            {
                this.playerHat.SetVisibility(true);
                InstallHatStats(null);
            }
            this.playerHat = playerHat;
            OnHatSet?.Invoke(playerHat);
        }
    }

    public void InstallHatStats(HatStats inHatStats)
    {
        if (inHatStats == hatStats)
        {
            return;
        }

        hatStats = inHatStats;

        if (spawnedMovementAbility)
        {
            Destroy(spawnedMovementAbility.gameObject);
            Destroy(spawnedMainAbility.gameObject);
        }

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

    public void OnMainActionStarted()
    {
        spawnedMainAbility.TryActivate();
    }

    public void OnMainActionCancelled()
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

    public HatStats GetHatStats()
    {
        return hatStats;
    }

    private void OnStatusEffectsChanged(List<StatusType> statuses)
    {
        if (playerHat)
        {
            playerHat.SetVisibility(!statuses.Contains(StatusType.HatInactive));
        }
    }
}