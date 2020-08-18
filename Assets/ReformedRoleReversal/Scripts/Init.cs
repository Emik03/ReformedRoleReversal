internal class Init
{
    internal Init(ReformedRoleReversal roleReversal)
    {
        RoleReversal = roleReversal;
        HandleManual = new HandleManual(this);
        _interact = new Interact(this);
    }

    /// <summary>
    /// First dimension represents the tutorial and 3 through 9 wires, while second dimension correspond through conditions 1 through 7.
    /// </summary>
    protected internal readonly Condition[,] Conditions = new Condition[8, 7];

    protected internal readonly ReformedRoleReversal RoleReversal;
    protected internal readonly HandleManual HandleManual;

    protected internal bool IsSolved = false, LightsOn = false;
    protected internal int ModuleId = 0, CorrectAnswer;
    protected internal static int ModuleIdCounter = 1;
    protected internal string Seed = string.Empty;

    private readonly Interact _interact;

    /// <summary>
    /// Initalizes the module.
    /// </summary>
    protected internal void Activate()
    {
        HandleManual.FormatSeed();

        RoleReversal.Screen.OnInteract += delegate ()
        {
            _interact.PressScreen();
            return false;
        };

        RoleReversal.Screen.OnInteractEnded += delegate ()
        {
            _interact.ReleaseScreen();
        };

        for (int i = 0; i < RoleReversal.Buttons.Length; i++)
        {
            int j = i;
            RoleReversal.Buttons[i].OnInteract += delegate ()
            {
                _interact.PressButton(j);
                return false;
            };
        }
    }
}
