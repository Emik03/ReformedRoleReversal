internal class Init
{
    internal Init(ReformedRoleReversal roleReversal)
    {
        RoleReversal = roleReversal;
        _handleManual = new HandleManual(this);
        _interact = new Interact(this);
    }

    protected internal ReformedRoleReversal RoleReversal;
    protected internal bool IsSolved = false, LightsOn = false;
    protected internal int ModuleId = 0;

    private HandleManual _handleManual;
    private Interact _interact;
    private static int _moduleIdCounter = 1;

    protected internal void Activate()
    {
        _handleManual.GenerateSeed();
        LightsOn = true;
        ModuleId = _moduleIdCounter++;

        RoleReversal.Screen.OnInteract += delegate ()
        {
            _interact.HandleScreen();
            return false;
        };

        for (int i = 0; i < RoleReversal.Buttons.Length; i++)
        {
            int j = i;
            RoleReversal.Buttons[i].OnInteract += delegate ()
            {
                _interact.HandleButtons(j);
                return false;
            };
        }
    }
}
