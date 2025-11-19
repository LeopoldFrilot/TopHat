using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    [SerializeField] private bool enableHitstop = true;
    
    private float recoverRate = .1f;
    private List<Coroutine> hitStops = new();
    private WaitForSecondsRealtime recoveryWait = new WaitForSecondsRealtime(.01f);

    private void Awake()
    {
        StartCoroutine(RecoverFromHitstop());
    }

    private IEnumerator RecoverFromHitstop()
    {
        while (Application.isPlaying)
        {
            if (Time.timeScale < 1 && Time.timeScale != 0)
            {
                Time.timeScale = Mathf.Clamp01(Time.timeScale + recoverRate);
            }
            yield return recoveryWait;
        }
    }

    public bool HasHitStop()
    {
        foreach (Coroutine coroutine in hitStops)
        {
            if (coroutine != null)
            {
                return true;
            }
        }
        
        return false;
    }
    
    public void AddStop(float duration, float recoverRate = -1f)
    {
        if (!enableHitstop)
        {
            return;
        }

        if (duration == 0)
        {
            return;
        }
        
        int newHandle = hitStops.Count;
        Coroutine newWait = StartCoroutine(StopForTime(duration, newHandle, recoverRate));
        hitStops.Add(newWait);
    }

    private IEnumerator StopForTime(float duration, int handle, float recoverRate)
    {
        Pause();
        yield return new WaitForSecondsRealtime(duration);
        hitStops[handle] = null;
        TryResume(recoverRate);
    }

    private void Pause()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
        }
    }

    private void TryResume(float recoverRate)
    {
        if (Time.timeScale == 0 && !HasHitStop())
        {
            Time.timeScale = recoverRate < 0 ? 1f : .001f;
            this.recoverRate = recoverRate;
        }
    }
}