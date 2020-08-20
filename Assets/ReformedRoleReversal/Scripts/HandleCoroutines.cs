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
        StartCoroutine(_handleManual.GenerateCondition(i, j, wires, strSeed));
    }

    internal protected IEnumerator RenderScreen(int instructionX, int instructionY, int wireSelected)
    {
        string currentText = string.Empty;
        _halt = true;
        
        string text = string.Format("Wire: {0}, Seed: {1}\n[{2}{3}]\n\n{4}",
                                    wireSelected,
                                    _init.Seed,
                                    instructionX == 0 ? "Tutorial" : (instructionX + 2).ToString() + " wires' ",
                                    instructionX == 0 ? string.Empty : StaticArrays.Ordinals[instructionY] + " condition",
                                    Algorithms.AddLineBreakPlaceholders(_init.Conditions[instructionX, instructionY].Text));

        yield return new WaitForSeconds(0.04f);

        _halt = false;
        RoleReversal.Text.text = string.Empty;

        for (int i = 0; i < text.Length; i++)
        {
            currentText += text[i] == '|' ? "\n" : text[i].ToString();

            RoleReversal.Text.text = _previousText.Length - currentText.Length >= 0
                                    ? currentText + "\n" + _previousText.Substring(currentText.Length, _previousText.Length - currentText.Length)
                                    : currentText;

            if (i % 3 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        for (int j = 0; RoleReversal.Text.text.Length > currentText.Length; j++)
        {
            RoleReversal.Text.text = RoleReversal.Text.text.Substring(0, RoleReversal.Text.text.Length - 2);

            if (j % 3 == 0 && !_halt)
                yield return new WaitForSeconds(0.02f);
        }

        RoleReversal.Text.text = currentText;
        _previousText = currentText;
    }
}
