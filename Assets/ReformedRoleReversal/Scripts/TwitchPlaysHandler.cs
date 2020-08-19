using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

internal class TwitchPlaysHandler
{
    internal TwitchPlaysHandler(Init init)
    {
        _init = init;
    }

    private Init _init;

    protected internal IEnumerator ProcessCommand(string command)
    {
        string[] parameters = command.Split(' ');

        // If the initial command is formatted correctly.
        if (Regex.IsMatch(parameters[0], @"^\s*cut\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            // If the command has incorrect amount of parameters.
            if (parameters.Length != 2)
                yield return parameters.Length < 2 ? "sendtochaterror Please specify the wire you want to cut! (Valid: 1-10)"
                                                   : "sendtochaterror Too many wires requested! Only one can be cut at any time.";

            // If the command has an invalid parameter.
            else if (parameters[1].Length == 1 && char.IsDigit(parameters[1][0]) && parameters[1][0] != '0')
                yield return "sendtochaterror Invalid number! Only wires 1-9 can be pushed.";

            // If the command is valid, cut wire accordingly.
            else
            {
                byte num = (byte)char.GetNumericValue(parameters[1][0]);
                while (num != _init.WireSelected)
                {
                    _init.RoleReversal.Buttons[3].OnInteract();
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }

        // If the initial command is formatted correctly.
        if (Regex.IsMatch(parameters[0], @"^\s*manual\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            // If the command has incorrect amount of parameters.
            if (parameters.Length != 2)
                yield return parameters.Length < 2 ? "sendtochaterror Please specify the wire you want to cut! (Valid: 1-10)"
                                                   : "sendtochaterror Too many wires requested! Only one can be cut at any time.";

            // If the command has an invalid parameter.
            else if (!IsValid(parameters[1], false))
                yield return "sendtochaterror Invalid instruction! Expected: <#>.<#>, 2-7.1-8";

            // If the command is valid, go to the respective part of the manual accordingly.
            else
            {
            }
        }
    }

    /// <summary>
    /// Force the module to be solved in TwitchPlays
    /// </summary>
    protected internal IEnumerator ForceSolve()
    {
        yield return null;
        Debug.LogFormat("[Role Reversal #{0}] A forced solve has been initiated...", _init.ModuleId);
    }
}
