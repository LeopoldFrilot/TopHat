using System;
using UnityEngine.Windows;

public enum THInputs
{
    Left,
    Right,
    Up,
    Down,
    Action,
    None
}

public class THInputManager
{
    private long previousInput;
    
    public void ParseInputs(long input)
    {
        CheckInput(input, THConstants.INPUT_LEFT);
        CheckInput(input, THConstants.INPUT_RIGHT);
        CheckInput(input, THConstants.INPUT_UP);
        CheckInput(input, THConstants.INPUT_DOWN);
        CheckInput(input, THConstants.INPUT_ACTION);
    }

    private void CheckInput(long currentInputCollection, int inputToCheck)
    {
        THInputs enumEquivalent;
        if (inputToCheck == THConstants.INPUT_LEFT)
        {
            enumEquivalent = THInputs.Left;
        }
        else if (inputToCheck == THConstants.INPUT_RIGHT)
        {
            enumEquivalent = THInputs.Right;
        }
        else if (inputToCheck == THConstants.INPUT_UP)
        {
            enumEquivalent = THInputs.Up;
        }
        else if (inputToCheck == THConstants.INPUT_DOWN)
        {
            enumEquivalent = THInputs.Down;
        }
        else if (inputToCheck == THConstants.INPUT_ACTION)
        {
            enumEquivalent = THInputs.Action;
        }
        else
        {
            enumEquivalent = THInputs.None;
        }
        
        if (ContainsInput(currentInputCollection, inputToCheck))
        {
            if (ContainsInput(previousInput, inputToCheck))
            {
                TriggerInputHeld(enumEquivalent);
            }
            else
            {
                TriggerInputStarted(enumEquivalent);
            }
        }
        else
        {
            if (ContainsInput(previousInput, inputToCheck))
            {
                TriggerInputEnded(enumEquivalent);
            }
        }
    }

    private bool ContainsInput(long inputCollection, int inputToCheck)
    {
        return (inputCollection & inputToCheck) != 0;
    }

    public Action<THInputs> OnInputStarted;
    public void TriggerInputStarted(THInputs input)
    {
        OnInputStarted?.Invoke(input);
    }
    
    public Action<THInputs> OnInputHeld;
    public void TriggerInputHeld(THInputs input)
    {
        OnInputHeld?.Invoke(input);
    }
    
    public Action<THInputs> OnInputEnded;
    public void TriggerInputEnded(THInputs input)
    {
        OnInputEnded?.Invoke(input);
    }
}