using System.Collections;
using UnityEngine;

/// <summary>
/// Handles all coroutines of the module since Unity enforces coroutines be started by a GameObject.
/// </summary>
public class HandleCoroutines : MonoBehaviour
{
    public ReformedRoleReversal Reversal;

    private HandleManual manual;
    private Init init;

    private string previous = string.Empty;
    private bool halt, freeze;

    private void Start()
    {
        init = Reversal.Init;
        manual = init.Manual;
    }

    /// <summary>
    /// Generates 1 set of conditions from the number of wires provided.
    /// </summary>
    /// <param name="i">The index of the first dmiension.</param>
    /// <param name="wires">The values of the wires.</param>
    /// <param name="strSeed">The seed in base-10.</param>
    /// <param name="lookup">The lookup table, which might be needed to revert values.</param>
    internal void GenerateSetOfConditions(int i, int[] wires, ref int lookup)
    {
        int j2 = init.Conditions.GetLength(1);
        for (int j = 0; j < j2; j++)
            StartCoroutine(manual.GenerateCondition(i, j, wires, lookup, i + 2 == wires.Length));
    }

    /// <summary>
    /// Starts the coroutine of animating the screen.
    /// </summary>
    /// <param name="instructionX">The index of the first dimension.</param>
    /// <param name="instructionY">The index of the second dimension.</param>
    /// <param name="wireSelected">The current selected wire.</param>
    /// <param name="isSelectingWire">Whether the module is in submission mode.</param>
    internal void UpdateScreen(int instructionX, int instructionY, ref int wireSelected, ref bool isSelectingWire)
    {
        StartCoroutine(RenderScreen(instructionX, instructionY, wireSelected, isSelectingWire));
    }

    /// <summary>
    /// Renders the screen with animating by displaying the text 1 character at a time.
    /// </summary>
    /// <param name="instructionX">The index of the first dimension.</param>
    /// <param name="instructionY">The index of the second dimension.</param>
    /// <param name="wireSelected">The current selected wire.</param>
    /// <param name="isSelectingWire">Whether the module is in submission mode.</param>
    /// <returns>It's an animation, it only returns WaitForSeconds().</returns>
    internal protected IEnumerator RenderScreen(int instructionX, int instructionY, int wireSelected, bool isSelectingWire)
    {
        string text;

        // Either state it is solved, show the currently submitted wire, or display the manual.
        if (init.Solved)
            text = string.Format("[Reformed Role Reversal #{0}]\n\nThe correct wire was cut.\nModule solved!", init.ModuleId);

        else if (isSelectingWire)
            text = string.Format("[Wire Selected: {0}]\n\nPlease press the screen\nto cut the wire.", wireSelected);

        else
            text = string.Format("[{0}{1}]\n\n{2}",
                                 instructionX == 0 ? "Tutorial" : (instructionX + 2).ToString() + " wires, ",
                                 instructionX == 0 ? string.Empty : Arrays.Ordinals[instructionY] + " condition",
                                 Algorithms.Format(init.Conditions[instructionX, instructionY].Text));


        if (!isSelectingWire || init.Solved)
            freeze = false;
        halt = true;

        // This delay should always be twice as much to make sure that an already running coroutine will halt.
        // StopCoroutine() doesn't appear to work, so this is a workaround.
        yield return new WaitForSeconds(0.04f);

        halt = false;
        Reversal.ScreenText.text = string.Empty;
        string current = string.Empty;

        // Display previous message as it is getting replaced.
        if (!init.Solved && !isSelectingWire)
            for (int i = 0; i < text.Length; i++)
            {
                current += text[i].ToString();

                // The substring for the previous instruction might be negative, so only 'currentText' is used instead in that case.
                Reversal.ScreenText.text = previous.Length - current.Length >= 0
                                         ? current + "\n" + previous.Substring(current.Length, previous.Length - current.Length)
                                         : current;

                // This makes characters display 2 at a time.
                if (i % 2 == 0 && !halt && !freeze)
                    yield return new WaitForSeconds(0.02f);
            }

        // Instantly clear the previous message.
        else
            for (int i = 0; i < text.Length; i++)
            {
                current += text[i].ToString();
                Reversal.ScreenText.text = current;

                // This makes characters display 2 at a time.
                if (i % 2 == 0 && !halt && !freeze)
                    yield return new WaitForSeconds(0.02f);
            }

        for (int j = 0; Reversal.ScreenText.text.Length > current.Length; j++)
        {
            // Remove any additional characters if the previous instruction was longer than the current.
            Reversal.ScreenText.text = Reversal.ScreenText.text.Substring(0, Reversal.ScreenText.text.Length - 2);

            // This makes characters display 2 at a time.
            if (j % 2 == 0 && !halt)
                yield return new WaitForSeconds(0.02f);
        }

        freeze = isSelectingWire;

        // Sometimes it removes the last character, this makes it reappear.
        Reversal.ScreenText.text = current;
        previous = current;
    }
}
