using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class TwitchPlaysHandler : MonoBehaviour
{
    private Init _init;
    private ReformedRoleReversal _roleReversal;

#pragma warning disable 414
    private const string TwitchHelpMessage = @"!{0} cut <#> (Cuts the wire '#' | valid numbers are from 1-7) !{0} manual <#>.<#> (Left digit refers to amount of wires, right digit refers to instruction count. If you don't know how this module works, do manual 1:3, manual 1:4, manual 1:5...)";
#pragma warning restore 414

    private void Start()
    {
        _roleReversal = GetComponent<ReformedRoleReversal>();
        _init = _roleReversal.Init;
    }

    private IEnumerator ProcessTwitchCommand(string command)
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
                    _roleReversal.Buttons[3].OnInteract();
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }

        // If the initial command is formatted correctly.
        if (Regex.IsMatch(parameters[0], @"^\s*manual\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return "detonate";
            // If the command has incorrect amount of parameters.
            if (parameters.Length != 2)
                yield return parameters.Length < 2 ? "sendtochaterror Please specify the wire you want to cut! (Valid: 1-10)"
                                                   : "sendtochaterror Too many wires requested! Only one can be cut at any time.";

            // If the command has an invalid parameter.
            //else if (!IsValid(parameters[1], false))
                //yield return "sendtochaterror Invalid instruction! Expected: <#>.<#>, 2-7.1-8";

            // If the command is valid, go to the respective part of the manual accordingly.
            else
            {
            }
        }
    }

    /// <summary>
    /// Force the module to be solved in TwitchPlays
    /// </summary>
    private IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
        Debug.LogFormat("[Role Reversal #{0}] A forced solve has been initiated...", _init.ModuleId);
    }
}
