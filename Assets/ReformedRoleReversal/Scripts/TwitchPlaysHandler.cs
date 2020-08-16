using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

internal class TwitchPlaysHandler
{
    internal TwitchPlaysHandler(Init init)
    {
        Init = init;
    }

    private Init Init;

    /// <summary>
    /// Determines whether the input from the TwitchPlays chat command is valid or not.
    /// </summary>
    /// <param name="par">The string from the user.</param>
    protected internal bool IsValid(string par, bool submit)
    {
        byte b;
        // Cut wire 1-7.
        if (submit)
            return byte.TryParse(par, out b) && b < 8 && b != 0;

        // Wire 1-7 (1 is tutorial), condition 0-8 (0 is section header)
        if (par.Length != 3)
            return false;

        return char.GetNumericValue(par.ToCharArray()[0]) < 8 && char.GetNumericValue(par.ToCharArray()[0]) != 0 && par.ToCharArray()[1] == '.' && char.GetNumericValue(par.ToCharArray()[2]) != 0 && char.GetNumericValue(par.ToCharArray()[2]) < 9;
    }

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
            else if (!IsValid(parameters[1], true))
                yield return "sendtochaterror Invalid number! Only wires 1-7 can be pushed.";

            // If the command is valid, cut wire accordingly.
            else
            {
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
        Debug.LogFormat("[Role Reversal #{0}] A forced solve has been initiated...", Init.ModuleId);
    }
}
