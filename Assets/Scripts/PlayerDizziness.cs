using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDizziness : MonoBehaviour
{
    
    private float dizzy;
    private float nextTimeToRecover;
    private Fighter fighterRef;

    private void Awake()
    {
        fighterRef = GetComponent<Fighter>();
    }

    private void Start()
    {
        ResetDizzy();
    }

    private void Update()
    {
        if (Time.time >= nextTimeToRecover)
        {
            SetNewStature(dizzy - Time.deltaTime * Help.Tunables.dizzyRecoveryRate);
        }
    }

    public void ResetDizzy()
    {
        SetNewStature(0);
        nextTimeToRecover = 0;
    }

    public void DealDizzyDamage(float damage)
    {
        if (damage > 0)
        {
            SetNewStature(dizzy + damage);
            ResetRecoveryTimer();
        }
    }

    private void SetNewStature(float newDizzy)
    {
        float prevStature = dizzy;
        newDizzy = Mathf.Clamp(newDizzy, 0f, Help.Tunables.maxDizziness);
        dizzy = newDizzy;
        if (prevStature != dizzy)
        {
            TriggerDizzinessChanged(dizzy);
        }
    }

    private void ResetRecoveryTimer()
    {
        nextTimeToRecover = Time.time + Help.Tunables.delayBeforeRecovery;
    }

    public Action<float> OnDizzinessChanged;
    private void TriggerDizzinessChanged(float newStature)
    {
        float dizzinessNormalized = newStature / Help.Tunables.maxDizziness;
        OnDizzinessChanged?.Invoke(dizzinessNormalized);
    }

    public float GetNormalizedDizziness()
    {
        return Mathf.Clamp01(dizzy/Help.Tunables.maxDizziness);
    }
}