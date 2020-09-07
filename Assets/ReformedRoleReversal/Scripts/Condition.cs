/// <summary>
/// Stores a condition for display in the module, and for keeping track whether the condition is true, and what to do if it is.
/// </summary>
sealed class Condition
{
    /// <summary>
    /// If the condition is true, the wire to cut gets stored here.
    /// </summary>
    public int? Wire { get; set; }

    /// <summary>
    /// If the condition is true, the condition to skip to gets stored here.
    /// </summary>
    public int? Skip { get; set; }

    /// <summary>
    /// If the condition is true, the amount of wires to remove gets stored here.
    /// IMPORTANT: This value being negative means leftmost wires, and positive rightmost!
    /// </summary>
    public int? Remove { get; set; }

    /// <summary>
    /// The text from the condition that will be displayed on the module's screen.
    /// </summary>
    public string Text { get; set; }
}
