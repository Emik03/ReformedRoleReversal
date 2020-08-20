using System.Diagnostics;

internal class Init
{
    internal Init(HandleCoroutines coroutines, ReformedRoleReversal roleReversal)
    {
        RoleReversal = roleReversal;

        Coroutines = coroutines;
        HandleManual = new HandleManual(coroutines, this);
        Interact = new Interact(this);
    }

    /// <summary>
    /// First dimension represents the tutorial and 3 through 9 wires, while second dimension correspond through conditions 1 through 8.
    /// </summary>
    protected internal readonly Condition[,] Conditions = new Condition[8, 8];

    protected internal readonly HandleCoroutines Coroutines;
    protected internal readonly HandleManual HandleManual;
    protected internal readonly Interact Interact;
    protected internal readonly ReformedRoleReversal RoleReversal;

    protected internal bool IsSolved = false, LightsOn = false;
    protected internal int ModuleId = 0, WireSelected = 1;
    protected internal int? CorrectAnswer;
    protected internal static int ModuleIdCounter = 1;
    protected internal string Seed = string.Empty;

    /// <summary>
    /// Initalizes the module.
    /// </summary>
    protected internal void Activate()
    {
        HandleManual.Generate();

        RoleReversal.Screen.OnInteract += delegate ()
        {
            Interact.PressScreen();
            return false;
        };

        RoleReversal.Screen.OnInteractEnded += delegate ()
        {
            Interact.ReleaseScreen();
        };

        for (int i = 0; i < RoleReversal.Buttons.Length; i++)
        {
            int j = i;
            RoleReversal.Buttons[i].OnInteract += delegate ()
            {
                Interact.PressButton(j);
                return false;
            };
        }
    }
}
