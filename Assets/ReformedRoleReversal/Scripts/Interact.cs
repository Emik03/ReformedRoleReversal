internal class Interact
{
    internal Interact(Init init)
    {
        Init = init;
    }

    private Init Init;

    /// <summary>
    /// Cycles through various bits of information depending on which button was pushed.
    /// </summary>
    /// <param name="num">The index for the 4 buttons so the program can differentiate which button was pushed.</param>
    protected internal void HandleButtons(int num)
    {
        // Plays button sound effect.
        Init.RoleReversal.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Init.RoleReversal.Buttons[num].transform);
        Init.RoleReversal.Buttons[num].AddInteractionPunch();

        // If lights are off, the buttons should do nothing.
        if (!Init.LightsOn || Init.IsSolved)
            return;


    }

    /// <summary>
    /// Registers whether the answer provided by the player is correct or not.
    /// </summary>
    protected internal void HandleScreen()
    {

    }
}
