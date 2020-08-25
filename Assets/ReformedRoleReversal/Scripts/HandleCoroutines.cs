using System.Collections;
using UnityEngine;

public class HandleCoroutines : MonoBehaviour
{
    public ReformedRoleReversal RoleReversal;

    private HandleManual _handleManual;
    private Init _init;

    private string _previousText = string.Empty;
    private bool _halt;

    private void Start()
    {
        _init = RoleReversal.Init;
        _handleManual = RoleReversal.Init.HandleManual;
    }

    internal void UpdateScreen(int instructionX, int instructionY, ref int wireSelected, ref bool isSelectingWire)
    {
        StartCoroutine(RenderScreen(instructionX, instructionY, wireSelected, isSelectingWire));
    }

    internal void GenerateCondition(int i, int j, int[] wires, ref string strSeed, ref int lookup)
    {
        StartCoroutine(_handleManual.GenerateCondition(i, j, wires, strSeed, lookup, i + 2 == wires.Length));
    }

    internal protected IEnumerator RenderScreen(int instructionX, int instructionY, int wireSelected, bool isSelectingWire)
    {
        string currentText = string.Empty;
        _halt = true;


        string text = isSelectingWire ? string.Format("[Wire Selected: {0}]\n\nPlease press the screen to\ncut the wire.", wireSelected) 
                                      : string.Format("[{0}{1}]\n\n{2}",
                                      instructionX == 0 ? "Tutorial" : (instructionX + 2).ToString() + " wires, ",
                                      instructionX == 0 ? string.Empty : StaticArrays.Ordinals[instructionY] + " condition",
                                      Algorithms.LineBreaks(_init.Conditions[instructionX, instructionY].Text));

        yield return new WaitForSeconds(0.04f);

        _halt = false;
        RoleReversal.ScreenText.text = string.Empty;

        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i] == '|' ? "\n" : text[i].ToString();

            RoleReversal.ScreenText.text = _previousText.Length - currentText.Length >= 0
                                    ? currentText + "\n" + _previousText.Substring(currentText.Length, _previousText.Length - currentText.Length)
                                    : currentText;

            if (i % 2 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        for (int j = 0; RoleReversal.ScreenText.text.Length > currentText.Length; j++)
        {
            RoleReversal.ScreenText.text = RoleReversal.ScreenText.text.Substring(0, RoleReversal.ScreenText.text.Length - 2);

            if (j % 2 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        RoleReversal.ScreenText.text = currentText;
        _previousText = currentText;
    }
}
