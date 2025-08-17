using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDizziness : MonoBehaviour
{
    [SerializeField] private float maxDizziness = 5f;
    [SerializeField] private float dizzyRecoveryRate = 10f;
    [SerializeField] private float delayBeforeRecovery = 0.5f;
    
    private float dizzy;
    private float nextTimeToRecover;

    private void Start()
    {
        ResetDizzy();
    }

    private void Update()
    {
        if (Time.time >= nextTimeToRecover)
        {
            SetNewStature(dizzy - Time.deltaTime * dizzyRecoveryRate);
        }
    }

    private void ResetDizzy()
    {
        SetNewStature(0);
        nextTimeToRecover = 0;
    }

    public void DealDizzyDamage(float damage)
    {
        if (damage > 0)
        {
            Debug.Log($"Dealing damage: {damage}");
            SetNewStature(dizzy + damage);
            ResetRecoveryTimer();
        }
    }

    private void SetNewStature(float newStature)
    {
        float prevStature = dizzy;
        newStature = Mathf.Clamp(newStature, 0f, maxDizziness);
        dizzy = newStature;
        if (prevStature != dizzy)
        {
            TriggerDizzinessChanged(dizzy);
        }
    }

    private void ResetRecoveryTimer()
    {
        nextTimeToRecover = Time.time + delayBeforeRecovery;
    }

    public Action<float> OnDizzinessChanged;
    private void TriggerDizzinessChanged(float newStature)
    {
        float dizzinessNormalized = newStature / maxDizziness;
        OnDizzinessChanged?.Invoke(dizzinessNormalized);
    }

    public float GetNormalizedDizziness()
    {
        return dizzy/maxDizziness;
    }
}