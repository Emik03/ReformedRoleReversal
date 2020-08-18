using System.Collections;
using UnityEngine;

internal class Animate
{
    internal Animate(ReformedRoleReversal roleReversal)
    {
        RoleReversal = roleReversal;
    }

    private readonly ReformedRoleReversal RoleReversal;

    internal protected IEnumerator UpdateScreen(string text, int instructionX, int instructionY, int wireSelected)
    {
        text = string.Format("[Selected Wire {0}]\n[{1} Wires, Condition {2}]:\n\n{3}", wireSelected, instructionX + 3, instructionY, Algorithms.AddLineBreakPlaceholders(text));
        RoleReversal.Text.text = "";

        for (int i = 0; i < text.Length; i++)
        {
            RoleReversal.Text.text += text[i] == '|' ? "\n" : text[i].ToString();
            yield return new WaitForSeconds(0.01f);
        }
    }
}
