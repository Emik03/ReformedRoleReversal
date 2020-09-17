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
    internal void GenerateSetOfConditions(int i, int[] wires, ref int lookup, ref bool discard, ref bool append)
    {
        int j2 = init.Conditions.GetLength(1);
        for (int j = 0; j < j2; j++)
            StartCoroutine(manual.GenerateCondition(i, j, wires, lookup, discard, append, i + 2 == wires.Length));
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

        // Either state it is solved...
        if (init.Solved)
            text = init.Interact.CorrectAnswer == null ? string.Format("[Reformed Role Reversal #{0}]\n\nAn internal error has occured\nwhilst trying to calculate the\nanswer. Module solved!", init.ModuleId % 10000)
                                                       : string.Format("[Reformed Role Reversal #{0}]\n\nThe correct wire was cut.\nModule solved!", init.ModuleId % 10000);

        // ...show the currently submitted wire...
        else if (isSelectingWire)
            text = string.Format("[Wire Selected: {0}]\n\nPlease press the screen\nto cut the wire.", wireSelected);

        // ...or display the manual.
        else
            text = string.Format("[{0}{1}]\n\n{2}", instructionX == 0 ? "Tutorial" : (instructionX + 2).ToString() + " wires, ", instructionX == 0 ? string.Empty : Arrays.Ordinals[instructionY] + " condition", Algorithms.Format(init.Conditions[instructionX, instructionY].Text));

        halt = true;

        // This delay should always be as much as the delay below to make sure that an already running coroutine will halt.
        // StopCoroutine() doesn't appear to work, so this is a workaround.
        const float wait = 0.025f;
        yield return new WaitForSeconds(wait);

        halt = false;

        // If you are selecting the wire, the animation should not play.
        if (!isSelectingWire || init.Solved)
            freeze = false;

        bool keepAnimating;
        byte i = 0;
        string current = string.Empty;

        do
        {
            keepAnimating = false;
            
            // Cut to next line break.
            if (previous.IndexOf('\n') != -1)
            {
                previous = previous.Substring(previous.IndexOf('\n') + 1);
                keepAnimating = true;
            }

            // Copy current until a line break is met.
            for (; i < text.Length; i++)
            {
                current += text[i];

                // If last character is a line break, stop for the time being.
                if (text[i] == '\n')
                {
                    keepAnimating = true;
                    i++;
                    break;
                }
            }

            // If it needs to stop, instantly display text and stop running.
            if (halt || freeze)
            {
                Reversal.ScreenText.text = text;
                break;
            }

            // Combine both strings.
            Reversal.ScreenText.text = previous + current;
            yield return new WaitForSeconds(wait);
        } while (keepAnimating);

        // We put it after so that the first time the user selects a wire, it animates normally.
        freeze = isSelectingWire;
        previous = text + '\n';
    }
}
