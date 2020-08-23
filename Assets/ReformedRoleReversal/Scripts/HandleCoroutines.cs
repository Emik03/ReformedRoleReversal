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

    internal void UpdateScreen(int instructionX, int instructionY, int wireSelected)
    {
        StartCoroutine(RenderScreen(instructionX, instructionY, wireSelected));
    }

    internal void GenerateCondition(int i, int j, int[] wires, ref string strSeed)
    {
        StartCoroutine(_handleManual.GenerateCondition(i, j, wires, strSeed, i + 2 == wires.Length));
    }

    internal protected IEnumerator RenderScreen(int instructionX, int instructionY, int wireSelected)
    {
        string currentText = string.Empty;
        _halt = true;
        
        string text = string.Format("[{0}{1}]\n\n{2}",
                                    instructionX == 0 ? "Tutorial" : (instructionX + 2).ToString() + " wires, ",
                                    instructionX == 0 ? string.Empty : StaticArrays.Ordinals[instructionY] + " condition",
                                    Algorithms.LineBreaks(_init.Conditions[instructionX, instructionY].Text));

        yield return new WaitForSeconds(0.04f);

        _halt = false;
        
        RoleReversal.Texts[2].text = string.Empty;

        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i] == '|' ? "\n" : text[i].ToString();

            RoleReversal.Texts[2].text = _previousText.Length - currentText.Length >= 0
                                    ? currentText + "\n" + _previousText.Substring(currentText.Length, _previousText.Length - currentText.Length)
                                    : currentText;

            if (i % 2 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        for (int j = 0; RoleReversal.Texts[2].text.Length > currentText.Length; j++)
        {
            RoleReversal.Texts[2].text = RoleReversal.Texts[2].text.Substring(0, RoleReversal.Texts[2].text.Length - 2);

            if (j % 2 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        RoleReversal.Texts[2].text = currentText;
        _previousText = currentText;
    }
}
