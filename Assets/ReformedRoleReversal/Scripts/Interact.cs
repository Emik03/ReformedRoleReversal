using System.Collections;
using UnityEngine;

internal class Interact
{
    internal Interact(Init init)
    {
        Init = init;
    }

    private Init Init;
    private int _instruction, _wireSelected = 1;

    /// <summary>
    /// Cycles through the UI depending on the button pushed.
    /// </summary>
    /// <param name="num">The index for the 4 buttons.</param>
    protected internal void PressButton(int num)
    {
        // Plays button sound effect.
        Init.RoleReversal.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Init.RoleReversal.Buttons[num].transform);
        Init.RoleReversal.Buttons[num].AddInteractionPunch();

        // If lights are off or the module is solved, the buttons should do nothing.
        if (!Init.LightsOn || Init.IsSolved)
            return;

        switch (num)
        {
            // Subtract 1 from the current selected wire.
            case 0: _wireSelected = ((_wireSelected + 5) % 7) + 1; break;

            // Read next instruction.
            case 1: _instruction = (_instruction + 48) % 49; break;

            // Read previous instruction.
            case 2: _instruction = ++_instruction % 49; break;

            // Add 1 to the current selected wire.
            case 3: _wireSelected = (_wireSelected % 7) + 1; break;
        }

        Init.RoleReversal.UpdateScreen(Init.Conditions[_instruction / 7, _instruction % 7].Text, _instruction / 7, _instruction % 7, _wireSelected);
    }

    /// <summary>
    /// Stores how long the button was held for.
    /// </summary>
    protected internal void PressScreen()
    {

    }

    /// <summary>
    /// If the screen was pressed, skip around the instructions, otherwise submit the answer.
    /// </summary>
    protected internal void ReleaseScreen()
    {

    }

    
}
