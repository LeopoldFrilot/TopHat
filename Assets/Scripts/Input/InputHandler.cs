using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private float actionTimeStart = -1;
    private bool active = true;
    
    public event Action<float> OnVerticalInputChanged;
    public void Vertical(InputAction.CallbackContext ctx)
    {
        if (!active)
        {
            return;
        }

        OnVerticalInputChanged?.Invoke(ctx.ReadValue<float>());
    }

    public event Action<float> OnHorizontalInputChanged;
    public void Horizontal(InputAction.CallbackContext ctx)
    {
        if (!active)
        {
            return;
        }
        
        OnHorizontalInputChanged?.Invoke(ctx.ReadValue<float>());
    }

    public event Action OnActionStarted;
    public event Action<float> OnActionCancelled;
    public void ActionPerformed(InputAction.CallbackContext ctx)
    {
        if (!active)
        {
            return;
        }
        
        if (ctx.started)
        {
            actionTimeStart = Time.time;
            OnActionStarted?.Invoke();
        }
        else if (ctx.canceled)
        {
            OnActionCancelled?.Invoke(Time.time - actionTimeStart);
            actionTimeStart = -1;
        }
    }
    
    public float GetCurrentActionHoldTime()
    {
        return Time.time - actionTimeStart;
    }

    public void SetActive(bool newValue)
    {
        active = newValue;
        if (!active)
        {
            OnVerticalInputChanged?.Invoke(0);
            OnHorizontalInputChanged?.Invoke(0);
            if (actionTimeStart >= 0)
            {
                OnActionCancelled?.Invoke(Time.time - actionTimeStart);
                actionTimeStart = -1;
            }
        }
    }
}
