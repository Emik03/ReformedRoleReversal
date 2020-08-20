using System.Diagnostics;

internal class Interact
{
    internal Interact(Init init)
    {
        _coroutines = init.Coroutines;
        _init = init;
        _roleReversal = init.RoleReversal;
    }

    private readonly Coroutines _coroutines;
    private readonly Init _init;
    private readonly ReformedRoleReversal _roleReversal;
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private int _instruction;

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

        int length = _init.Conditions.GetLength(0) * _init.Conditions.GetLength(1);

        switch (num)
        {
            // Subtract 1 from the current selected wire.
            case 0: _init.WireSelected = ((_init.WireSelected + 7) % 9) + 1; break;

            // Read previous instruction.
            case 1: _instruction = (--_instruction + length) % length; break;

            // Read next instruction.
            case 2: _instruction = ++_instruction % length; break;

            // Add 1 to the current selected wire.
            case 3: _init.WireSelected = (_init.WireSelected % 9) + 1; break;
        }

        _coroutines.UpdateScreen(instructionX: _instruction / _init.Conditions.GetLength(1), instructionY: _instruction % _init.Conditions.GetLength(1), wireSelected: _init.WireSelected);
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

        _stopwatch.Reset();
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
            if (_init.WireSelected == _init.CorrectAnswer || _init.CorrectAnswer == 0)
            {
                UnityEngine.Debug.LogFormat("[Reformed Role Reversal #{0}]: The correct wire was cut. Module solved!", _init.ModuleId);
                _init.IsSolved = true;
                _init.RoleReversal.Module.HandlePass();
            }

            else
            {
                UnityEngine.Debug.LogFormat("[Reformed Role Reversal #{0}]: Wire {1} was cut which was incorrect. Module strike!", _init.ModuleId, _init.WireSelected);
                _init.RoleReversal.Module.HandleStrike();
            }
        }

        // Jump to next section.
        else
        {
            _instruction = ((_instruction / _init.Conditions.GetLength(1)) + 1) * _init.Conditions.GetLength(1) % _init.Conditions.GetLength(0) * _init.Conditions.GetLength(1);
            _coroutines.UpdateScreen(instructionX: _instruction / _init.Conditions.GetLength(1), instructionY: _instruction % _init.Conditions.GetLength(1), wireSelected: _init.WireSelected);
        }
    }
}
