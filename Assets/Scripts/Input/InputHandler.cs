using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : NetworkBehaviour
{
    private bool active = true;
    private NetworkedFighterController networkedFighterController;
    
    public event Action<float, int> OnVerticalInputChanged;
    public void PadVertical(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }

        OnVerticalInputChanged?.Invoke(ctx.ReadValue<float>(), 0);
    }

    public void KeyVertical(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }

        OnVerticalInputChanged?.Invoke(ctx.ReadValue<float>(), 0);
    }

    public void Key2Vertical(InputAction.CallbackContext ctx)
    {
        if (!IsActive())
        {
            return;
        }

        OnVerticalInputChanged?.Invoke(ctx.ReadValue<float>(), 1);
    }

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
            OnVerticalInputChanged?.Invoke(0, 1);
            OnHorizontalInputChanged?.Invoke(0, 1);
            OnActionCancelled?.Invoke(1);
        }
    }
}
