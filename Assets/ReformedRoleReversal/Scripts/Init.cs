internal class Init
{
    internal Init(ReformedRoleReversal roleReversal)
    {
        RoleReversal = roleReversal;
        _handleManual = new HandleManual(this);
        _interact = new Interact(this);
    }

    /// <summary>
    /// First dimension represents the tutorial and 3 through 9 wires, while second dimension correspond through conditions 1 through 7.
    /// </summary>
    protected internal readonly Condition[,] Conditions = new Condition[8, 7];

    protected internal readonly ReformedRoleReversal RoleReversal;
    protected internal bool IsSolved = false, LightsOn = false;
    protected internal int ModuleId = 0;

    private HandleManual _handleManual;
    private Interact _interact;
    private static int _moduleIdCounter = 1;

    /// <summary>
    /// Initalizes the module.
    /// </summary>
    protected internal void Activate()
    {
        _handleManual.FormatSeed();
        LightsOn = true;
        ModuleId = _moduleIdCounter++;

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
