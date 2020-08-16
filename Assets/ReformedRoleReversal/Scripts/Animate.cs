internal class Animate
{
    internal Animate(Init init)
    {
        Init = init;
    }

    private Init Init;

    protected internal void UpdateScreen()
    {
        Init.IsSolved = false;
    }
}
