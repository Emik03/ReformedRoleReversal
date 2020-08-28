/// <summary>
/// The center core of the module that calls other methods and classes.
/// </summary>
internal class Init
{
    internal Init(HandleCoroutines coroutines, ReformedRoleReversal reversal)
    {
        Reversal = reversal;
        Coroutines = coroutines;

        Interact = new Interact(this);
        Manual = new HandleManual(coroutines, this);
    }

    /// <summary>
    /// First dimension represents the tutorial and 3 through 9 wires, while second dimension correspond through conditions 1 through 8.
    /// </summary>
    protected internal readonly Condition[,] Conditions = new Condition[8, 8];

    protected internal readonly HandleCoroutines Coroutines;
    protected internal readonly HandleManual Manual;
    protected internal readonly Interact Interact;
    protected internal readonly ReformedRoleReversal Reversal;

    protected internal static bool LightsOn = false;
    protected internal bool IsSolved = false;
    protected internal static int ModuleIdCounter = 1;
    protected internal int ModuleId = 0;

    /// <summary>
    /// Initalizes the module.
    /// </summary>
    protected internal void Activate()
    {
        ModuleId = ModuleIdCounter++;

        Manual.Generate();

        Reversal.Screen.OnInteract += delegate ()
        {
            Interact.PressScreen();
            return false;
        };

        Reversal.Screen.OnInteractEnded += delegate ()
        {
            Reversal.Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonRelease, Reversal.Screen.transform);
            Reversal.Screen.AddInteractionPunch(3);
        };

        for (int i = 0; i < Reversal.Buttons.Length; i++)
        {
            int j = i;
            Reversal.Buttons[i].OnInteract += delegate ()
            {
                Interact.PressButton(ref j);
                return false;
            };
        }
    }
}
