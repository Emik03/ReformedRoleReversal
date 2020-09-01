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
        Module.OnActivate += (Init = new Init(Coroutines, this)).Activate;
    }
}
