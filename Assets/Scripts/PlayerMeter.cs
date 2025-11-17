using System;
using UnityEngine;

public class PlayerMeter : MonoBehaviour
{
    [SerializeField] private int maxMeter = 10;
    int accumulatedMeter = 0;

    public Action<int> OnMeterChanged;
    public void SetMeter(int newMeter)
    {
        newMeter = Mathf.Clamp(newMeter, 0, maxMeter);
        if (accumulatedMeter != newMeter)
        {
            accumulatedMeter = newMeter;
            OnMeterChanged?.Invoke(accumulatedMeter);
        }
    }

    public void ChangeMeter(int delta)
    {
        SetMeter(accumulatedMeter + delta);
    }

    public int GetMeter()
    {
        return accumulatedMeter;
    }

    public void ResetMeter()
    {
        SetMeter(0);
    }
}