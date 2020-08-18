using UnityEngine;
using System.Collections;

public class ReformedRoleReversal : MonoBehaviour
{
    public ReformedRoleReversal()
    {
        _animate = new Animate(this);
        Init = new Init(this);
        _handleManual = new HandleManual(Init);
        _tp = new TwitchPlaysHandler(Init);
    }

    public Component Background;
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMSelectable[] Buttons;
    public KMSelectable Screen;
    public TextMesh Text;

    internal readonly Init Init;

    private readonly Animate _animate;
    private readonly HandleManual _handleManual;
    private readonly TwitchPlaysHandler _tp;

    private void Start()
    {
        Module.OnActivate += Init.Activate;
    }

    internal void UpdateScreen(int instructionX, int instructionY, int wireSelected)
    {
        StartCoroutine(_animate.UpdateScreen(instructionX, instructionY, wireSelected));
    }

    internal void GenerateCondition(int i, int j, int[] wires)
    {
        StartCoroutine(_handleManual.GenerateCondition(i, j, wires));
    }

#pragma warning disable 414
    private const string TwitchHelpMessage = @"!{0} cut <#> (Cuts the wire '#' | valid numbers are from 1-7) !{0} manual <#>.<#> (Left digit refers to amount of wires, right digit refers to instruction count. If you don't know how this module works, do manual 1:3, manual 1:4, manual 1:5...)";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        StartCoroutine(_tp.ProcessCommand(command));
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        StartCoroutine(_tp.ForceSolve());
        yield return null;
    }
}
