public class THMovementManager
{
    private THInputManager inputManagerRef;
    private THPlayer playerRef;
    public THMovementManager(THPlayer player)
    {
        playerRef = player;
        inputManagerRef = player.inputmanager;
        inputManagerRef.OnInputStarted += OnInputStarted;
        inputManagerRef.OnInputHeld += OnInputHeld;
        inputManagerRef.OnInputEnded += OnInputEnded;
    }

    private void OnInputEnded(THInputs input)
    {
        throw new System.NotImplementedException();
    }

    private void OnInputHeld(THInputs input)
    {
        throw new System.NotImplementedException();
    }

    private void OnInputStarted(THInputs input)
    {
        throw new System.NotImplementedException();
    }
}