﻿using System.Collections.Generic;
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

    protected internal readonly Stopwatch Stopwatch = new Stopwatch();
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
                _init.WireSelected = ((_init.WireSelected + 7) % 9) + 1;
                _init.RoleReversal.Texts[1].text = "Wire: " + _init.WireSelected.ToString();
                break;

            // Read previous instruction.
            case 1:
                Instruction = (--Instruction + length) % length;
                _coroutines.UpdateScreen(instructionX: Instruction / _init.Conditions.GetLength(1), instructionY: Instruction % _init.Conditions.GetLength(1), wireSelected: _init.WireSelected);
                break;

            // Read next instruction.
            case 2:
                Instruction = ++Instruction % length;
                _coroutines.UpdateScreen(instructionX: Instruction / _init.Conditions.GetLength(1), instructionY: Instruction % _init.Conditions.GetLength(1), wireSelected: _init.WireSelected);
                break;

            // Add 1 to the current selected wire.
            case 3:
                _init.WireSelected = (_init.WireSelected % 9) + 1;
                _init.RoleReversal.Texts[1].text = "Wire: " + _init.WireSelected.ToString();
                break;
        }

        
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

        Stopwatch.Reset();
        Stopwatch.Start();
    }
    
    /// <summary>
    /// If the screen was pressed, skip around the instructions, otherwise submit the answer.
    /// </summary>
    protected internal void ReleaseScreen()
    {
        // Plays button sound effect.
        _roleReversal.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, _roleReversal.Screen.transform);
        _roleReversal.Screen.AddInteractionPunch(3);

        // If lights are off or the module is solved, the buttons should do nothing.
        if (!Init.LightsOn || _init.IsSolved)
            return;

        Stopwatch.Stop();

        // The wire gets cut here.
        if (Stopwatch.ElapsedMilliseconds > 500)
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
                _roleReversal.Module.HandleStrike();
            }
        }

        // Jump to next section.
        else
        {
            int lengthShort = _init.Conditions.GetLength(1), length = _init.Conditions.GetLength(0) * _init.Conditions.GetLength(1);
            Instruction = ((Instruction / lengthShort) + 1) * lengthShort % length;
            _coroutines.UpdateScreen(instructionX: Instruction / lengthShort, instructionY: Instruction % lengthShort, wireSelected: _init.WireSelected);
        }
    }
}
