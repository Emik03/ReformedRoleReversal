using System.Diagnostics;

internal class Interact
{
    internal Interact(Init init)
    {
        _init = init;
    }

    private readonly Init _init;
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private int _instruction, _wireSelected = 1;

    /// <summary>
    /// Cycles through the UI depending on the button pushed.
    /// </summary>
    /// <param name="num">The index for the 4 buttons.</param>
    protected internal void PressButton(int num)
    {
        // Plays button sound effect.
        _init.RoleReversal.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, _init.RoleReversal.Buttons[num].transform);
        _init.RoleReversal.Buttons[num].AddInteractionPunch();

        // If lights are off or the module is solved, the buttons should do nothing.
        if (!_init.LightsOn || _init.IsSolved)
            return;

        switch (num)
        {
            // Subtract 1 from the current selected wire.
            case 0: _wireSelected = ((_wireSelected + 5) % 7) + 1; break;

            // Read next instruction.
            case 1: _instruction = ++_instruction % 56; break;

            // Read previous instruction.
            case 2: _instruction = (_instruction + 55) % 56; break;

            // Add 1 to the current selected wire.
            case 3: _wireSelected = (_wireSelected % 7) + 1; break;
        }
        
        _init.RoleReversal.UpdateScreen(_init.Conditions[_instruction / 7, _instruction % 7].Text, _instruction / 7, _instruction % 7, _wireSelected);
    }

    /// <summary>
    /// Stores how long the button was held for.
    /// </summary>
    protected internal void PressScreen()
    {
        // Plays button sound effect.
        _init.RoleReversal.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, _init.RoleReversal.Screen.transform);
        _init.RoleReversal.Screen.AddInteractionPunch(3);

        // If lights are off or the module is solved, the buttons should do nothing.
        if (!_init.LightsOn || _init.IsSolved)
            return;

        _stopwatch.Start();
    }

    /// <summary>
    /// If the screen was pressed, skip around the instructions, otherwise submit the answer.
    /// </summary>
    protected internal void ReleaseScreen()
    {
        // Plays button sound effect.
        _init.RoleReversal.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, _init.RoleReversal.Screen.transform);
        _init.RoleReversal.Screen.AddInteractionPunch(3);

        // If lights are off or the module is solved, the buttons should do nothing.
        if (!_init.LightsOn || _init.IsSolved)
            return;

        _stopwatch.Stop();

        // The wire gets cut here.
        if (_stopwatch.ElapsedMilliseconds > 500)
        {
            _init.RoleReversal.Module.HandlePass();
        }

        // Jump to next section.
        else
        {
            _instruction = ((_instruction / 7) + 1) * 7 % 56;
            _init.RoleReversal.UpdateScreen(_init.Conditions[_instruction / 7, _instruction % 7].Text, _instruction / 7, _instruction % 7, _wireSelected);
        }

        _stopwatch.Reset();
    }
}
