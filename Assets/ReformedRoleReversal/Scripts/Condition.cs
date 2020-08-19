sealed class Condition
{
    /// <summary>
    /// If the condition is true, the wire to cut gets stored here.
    /// </summary>
    public int Wire { get; set; }

    /// <summary>
    /// If the condition is true, the condition to skip to gets stored here.
    /// </summary>
    public int SkipTo { get; set; }

    /// <summary>
    /// The text from the condition that will be displayed on the module's screen.
    /// </summary>
    public string Text { get; set; }
}
