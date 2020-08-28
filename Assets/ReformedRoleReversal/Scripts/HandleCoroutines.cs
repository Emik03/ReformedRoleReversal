using System.Collections;
using UnityEngine;

/// <summary>
/// Handles all coroutines of the module since Unity enforces coroutines be started by a GameObject.
/// </summary>
public class HandleCoroutines : MonoBehaviour
{
    public ReformedRoleReversal Reversal;

    private HandleManual _manual;
    private Init _init;
    private string _previousText = string.Empty;
    private bool _halt;

    private void Start()
    {
        _init = Reversal.Init;
        _manual = _init.Manual;
    }

    internal void GenerateCondition(int i, int j, int[] wires, ref string strSeed, ref int lookup)
    {
        StartCoroutine(_manual.GenerateCondition(i, j, wires, strSeed, lookup, i + 2 == wires.Length));
    }

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
    /// <param name="isSelectingWire">Whether or not the module is in submission mode.</param>
    /// <returns>It's an animation, it only returns WaitForSeconds().</returns>
    internal protected IEnumerator RenderScreen(int instructionX, int instructionY, int wireSelected, bool isSelectingWire)
    {
        // Either display the current condition in manual mode, or display the currently selected wire in submission mode.
        string text = isSelectingWire ? string.Format("[Wire Selected: {0}]\n\nPlease press the screen to\ncut the wire.", wireSelected) 
                                      : string.Format("[{0}{1}]\n\n{2}",
                                      instructionX == 0 ? "Tutorial" : (instructionX + 2).ToString() + " wires, ",
                                      instructionX == 0 ? string.Empty : Arrays.Ordinals[instructionY] + " condition",
                                      Algorithms.LineBreaks(_init.Conditions[instructionX, instructionY].Text));

        _halt = true;
        string currentText = string.Empty;

        // This delay should always be twice as much to make sure that an already running coroutine will halt.
        // StopCoroutine() doesn't appear to work, so this is a workaround.
        yield return new WaitForSeconds(0.04f);

        _halt = false;
        Reversal.ScreenText.text = string.Empty;

        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i].ToString();

            // The substring for the previous instruction might be negative, so only 'currentText' is used instead in that case.
            Reversal.ScreenText.text = _previousText.Length - currentText.Length >= 0
                                     ? currentText + "\n" + _previousText.Substring(currentText.Length, _previousText.Length - currentText.Length)
                                     : currentText;

            // This makes characters display 2 at a time.
            if (i % 2 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        for (int j = 0; Reversal.ScreenText.text.Length > currentText.Length; j++)
        {
            // Remove any additional characters if the previous instruction was longer than the current.
            Reversal.ScreenText.text = Reversal.ScreenText.text.Substring(0, Reversal.ScreenText.text.Length - 2);

            // This makes characters display 2 at a time.
            if (j % 2 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        // Sometimes it accidentally removes the last character, this makes it reappear.
        Reversal.ScreenText.text = currentText;
        _previousText = currentText;
    }
}
