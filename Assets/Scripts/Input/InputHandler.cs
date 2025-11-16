using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : NetworkBehaviour
{
    private bool active = true;
    private NetworkedFighterController networkedFighterController;

    public event Action<float, int> OnHorizontalInputChanged;
    public void PadHorizontal(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        OnHorizontalInputChanged?.Invoke(ctx.ReadValue<float>(), 0);
    }
    
    public void KeyHorizontal(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        OnHorizontalInputChanged?.Invoke(ctx.ReadValue<float>(), 0);
    }
    
    public void Key2Horizontal(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        OnHorizontalInputChanged?.Invoke(ctx.ReadValue<float>(), 1);
    }

    public event Action<int> OnActionStarted;
    public event Action<int> OnActionCancelled;
    public void PadActionPerformed(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        if (ctx.started)
        {
            OnActionStarted?.Invoke(0);
        }
        else if (ctx.canceled)
        {
            OnActionCancelled?.Invoke(0);
        }
    }
    
    public void KeyActionPerformed(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        if (ctx.started)
        {
            OnActionStarted?.Invoke(0);
        }
        else if (ctx.canceled)
        {
            OnActionCancelled?.Invoke(0);
        }
    }
    
    public void Key2ActionPerformed(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        if (ctx.started)
        {
            OnActionStarted?.Invoke(1);
        }
        else if (ctx.canceled)
        {
            OnActionCancelled?.Invoke(1);
        }
    }
    
    public event Action<int> OnUpActionStarted;
    public event Action<int> OnUpActionCancelled;
    public void PadUp(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        if (ctx.started)
        {
            OnUpActionStarted?.Invoke(0);
        }
        else if (ctx.canceled)
        {
            OnUpActionCancelled?.Invoke(0);
        }
    }

    public void KeyUp(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        if (ctx.started)
        {
            OnUpActionStarted?.Invoke(0);
        }
        else if (ctx.canceled)
        {
            OnUpActionCancelled?.Invoke(0);
        }
    }

    public void Key2Up(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        if (ctx.started)
        {
            OnUpActionStarted?.Invoke(0);
        }
        else if (ctx.canceled)
        {
            OnUpActionCancelled?.Invoke(0);
        }
    }
    
    public event Action<int> OnDownActionStarted;
    public event Action<int> OnDownActionCancelled;
    public void PadDown(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        if (ctx.started)
        {
            OnDownActionStarted?.Invoke(0);
        }
        else if (ctx.canceled)
        {
            OnDownActionCancelled?.Invoke(0);
        }
    }

    public void KeyDown(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        if (ctx.started)
        {
            OnDownActionStarted?.Invoke(0);
        }
        else if (ctx.canceled)
        {
            OnDownActionCancelled?.Invoke(0);
        }
    }

    public void Key2Down(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }
        
        if (ctx.started)
        {
            OnDownActionStarted?.Invoke(0);
        }
        else if (ctx.canceled)
        {
            OnDownActionCancelled?.Invoke(0);
        }
    }

    private bool IsActive()
    {
        if (!active)
        {
            return false;
        }

        if (GameWizard.Instance.IsNetworkedGame())
        {
            return IsOwner;
        }
        
        return true;
    }

    public void SetActive(bool newValue)
    {
        active = newValue;
        if (!active)
        {
            OnHorizontalInputChanged?.Invoke(0, 1);
            OnUpActionStarted?.Invoke(1);
            OnDownActionStarted?.Invoke(1);
            OnActionCancelled?.Invoke(1);
        }
    }
}
