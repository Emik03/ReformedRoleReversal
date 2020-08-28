using System.Collections.Generic;
using System.Diagnostics;

internal class Interact
{
    internal Interact(Init init)
    {
        _coroutines = init.Coroutines;
        _init = init;
        _roleReversal = init.RoleReversal;
    }

    private readonly HandleCoroutines _coroutines;
    private readonly Init _init;
    private readonly ReformedRoleReversal _roleReversal;
    private bool _isSelectingWire = false;
    
    protected internal int Instruction;
    protected internal List<int> ButtonOrder = new List<int>(4){ 0, 1, 2, 3 }.Shuffle();

    /// <summary>
    /// Cycles through the UI depending on the button pushed.
    /// </summary>
    /// <param name="num">The index for the 4 buttons.</param>
    protected internal void PressButton(ref int num)
    {
        // Plays button sound effect.
        _roleReversal.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, _roleReversal.Buttons[num].transform);
        _roleReversal.Buttons[num].AddInteractionPunch();

        // If lights are off or the module is solved, the buttons should do nothing.
        if (!Init.LightsOn || _init.IsSolved)
            return;

        int length = _init.Conditions.GetLength(0) * _init.Conditions.GetLength(1);

        switch (ButtonOrder[num])
        {
            // Subtract 1 from the current selected wire.
            case 0:
                if (_isSelectingWire)
                    _init.WireSelected = ((_init.WireSelected + 7) % 9) + 1;
                _isSelectingWire = true;
                break;

            // Read previous instruction.
            case 1:
                if (_isSelectingWire)
                    return;
                Instruction = (--Instruction + length) % length;
                break;

            // Read next instruction.
            case 2:
                if (_isSelectingWire)
                    return;
                Instruction = ++Instruction % length;
                break;

            // Add 1 to the current selected wire.
            case 3:
                if (_isSelectingWire)
                    _init.WireSelected = (_init.WireSelected % 9) + 1;
                _isSelectingWire = true;
                break;
        }

        _coroutines.UpdateScreen(instructionX: Instruction / _init.Conditions.GetLength(1), instructionY: Instruction % _init.Conditions.GetLength(1), wireSelected: ref _init.WireSelected, isSelectingWire: ref _isSelectingWire);
    }

    /// <summary>
    /// Stores how long the button was held for.
    /// </summary>
    protected internal void PressScreen()
    {
        // Plays button sound effect.
        _roleReversal.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, _roleReversal.Screen.transform);
        _roleReversal.Screen.AddInteractionPunch(3);

        // If lights are off or the module is solved, the buttons should do nothing.
        if (!Init.LightsOn || _init.IsSolved)
            return;

        // The wire gets cut here.
        if (_isSelectingWire)
        {
            if (_init.CorrectAnswer == null || _init.CorrectAnswer == _init.WireSelected)
            {
                UnityEngine.Debug.LogFormat("[Reformed Role Reversal #{0}]: The correct wire was cut. Module solved!", _init.ModuleId);
                _init.IsSolved = true;
                _roleReversal.Module.HandlePass();
            }

            else
            {
                UnityEngine.Debug.LogFormat("[Reformed Role Reversal #{0}]: Wire {1} was cut which was incorrect. Module strike!", _init.ModuleId, _init.WireSelected);
                _isSelectingWire = false;
                Instruction = 0;
                _coroutines.UpdateScreen(instructionX: Instruction / _init.Conditions.GetLength(1), instructionY: Instruction % _init.Conditions.GetLength(1), wireSelected: ref _init.WireSelected, isSelectingWire: ref _isSelectingWire);
                _roleReversal.Module.HandleStrike();
            }
        }

        // Jump to next section.
        else
        {
            int lengthShort = _init.Conditions.GetLength(1), length = _init.Conditions.GetLength(0) * _init.Conditions.GetLength(1);
            Instruction = ((Instruction / lengthShort) + 1) * lengthShort % length;
            _coroutines.UpdateScreen(instructionX: Instruction / lengthShort, instructionY: Instruction % lengthShort, wireSelected: ref _init.WireSelected, isSelectingWire: ref _isSelectingWire);
        }
    }
}
