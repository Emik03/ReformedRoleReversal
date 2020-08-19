using System.Collections;
using UnityEngine;

internal class Animate
{
    internal Animate(ReformedRoleReversal roleReversal)
    {
        _roleReversal = roleReversal;
    }
    
    private readonly ReformedRoleReversal _roleReversal;
    private string _previousText = string.Empty;
    private bool _halt;

    internal protected IEnumerator UpdateScreen(int instructionX, int instructionY, int wireSelected)
    {
        string currentText = string.Empty;
        _roleReversal.Text.text = string.Empty;
        _halt = true;

        string text = string.Format("Wire: {0}, Seed: {1}\n[{2}{3}]\n\n{4}", 
                                    wireSelected, 
                                    _roleReversal.Init.Seed, 
                                    instructionX == 0 ? "Tutorial" : (instructionX + 2).ToString() + " wires' ", 
                                    instructionX == 0 ? string.Empty : StaticArrays.Ordinals[instructionY] + " condition", 
                                    Algorithms.AddLineBreakPlaceholders(_roleReversal.Init.Conditions[instructionX, instructionY].Text));

        yield return new WaitForSeconds(0.02f);
        _halt = false;

        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i] == '|' ? "\n" : text[i].ToString();

            _roleReversal.Text.text = _previousText.Length - currentText.Length >= 0
                                    ? currentText + "\n" + _previousText.Substring(currentText.Length, _previousText.Length - currentText.Length)
                                    : currentText;

            if (i % 2 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        for (int j = 0; _roleReversal.Text.text.Length > currentText.Length; j++)
        {
            _roleReversal.Text.text = _roleReversal.Text.text.Substring(0, _roleReversal.Text.text.Length - 2);

            if (j % 2 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        _roleReversal.Text.text = currentText;
        _previousText = currentText;
    }
}
