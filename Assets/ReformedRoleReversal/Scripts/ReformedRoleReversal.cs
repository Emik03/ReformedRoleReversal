using UnityEngine;

public class ReformedRoleReversal : MonoBehaviour
{
    public HandleCoroutines Coroutines;
    public KMAudio Audio;
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMSelectable[] Buttons;
    public KMSelectable Screen;
    public TextMesh ScreenText;
    public TextMesh SeedText;

    internal Init Init;

    private void Awake()
    {
        Init = new Init(Coroutines, this);
        Module.OnActivate += Init.Activate;
    }
}
