using System;
using UnityEngine;

public class PlayerMeter : MonoBehaviour
{
    float accumulatedMeter = 0;

    public Action<float> OnMeterChanged;
    public void SetMeter(float newMeter)
    {
        newMeter = Mathf.Clamp(newMeter, 0, Help.Tunables.maxMeter);
        if (!Mathf.Approximately(accumulatedMeter, newMeter))
        {
            accumulatedMeter = newMeter;
            OnMeterChanged?.Invoke(accumulatedMeter);
        }
    }

    public void ChangeMeter(float delta)
    {
        SetMeter(accumulatedMeter + delta);
    }

    public float GetMeter()
    {
        return accumulatedMeter;
    }

    public void ResetMeter()
    {
        SetMeter(0);
    }
}