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
        if (fighterRef.IsStunned())
        {
            SetNewStature(maxDizziness);
            return;
        }
        
        if (Time.time >= nextTimeToRecover)
        {
            SetNewStature(dizzy - Time.deltaTime * dizzyRecoveryRate);
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
            Debug.Log($"Dealing damage: {damage}. Total Damage: {dizzy}");
            ResetRecoveryTimer();
        }
    }

    private void SetNewStature(float newDizzy)
    {
        float prevStature = dizzy;
        newDizzy = Mathf.Clamp(newDizzy, 0f, maxDizziness);
        dizzy = newDizzy;
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
        return Mathf.Clamp01(dizzy/maxDizziness);
    }
}