using KModkit;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains mostly static information and indexable edgework.
/// </summary>
internal class Arrays
{
    internal Arrays(KMBombInfo Info)
    {
        _info = Info;
    }

    private readonly KMBombInfo _info;
    private const string _version = "v1";

    private static readonly Indicator[] _indicators = new Indicator[11]
    {
        Indicator.BOB,
        Indicator.CAR,
        Indicator.CLR,
        Indicator.FRK,
        Indicator.FRQ,
        Indicator.IND,
        Indicator.MSA,
        Indicator.NSA,
        Indicator.SIG,
        Indicator.SND,
        Indicator.TRN
    };

    /// <summary>
    /// Indexable array of indicator edgework in alphabetical order.
    /// </summary>
    internal static Indicator[] Indicators
    {
        get { return _indicators; }
        set { Indicators = value; }
    }

    private static readonly string[] _indicatorNames = new string[11]
    {
        "a BOB",
        "a CAR",
        "a CLR",
        "an FRK",
        "an FRQ",
        "an IND",
        "an MSA",
        "an NSA",
        "an SIG",
        "an SND",
        "a TRN",
    };

    /// <summary>
    /// Indexable array of indicator string names in alphabetical order.
    /// </summary>
    internal static string[] IndicatorNames
    {
        get { return _indicatorNames; }
        set { IndicatorNames = value; }
    }

    private static readonly char[] _base62 = new char[62]
    {
        '0','1','2','3','4','5','6','7','8','9',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z' 
    };

    /// <summary>
    /// 62 length array consisting of 0-9, A-Z, a-z
    /// </summary>
    internal static char[] Base62
    {
        get { return _base62; }
        set { Base62 = value; }
    }

    private static readonly string[] _strings = new string[17]
    {
        "batteries",
        "AA batteries",
        "D batteries",
        "battery holders",
        "indicators",
        "lit indicators",
        "unlit indicators",
        "port plates",
        "unique ports",
        "duplicate ports",
        "ports",
        "numbers in the serial number",
        "letters in the serial number",
        "modules (excluding needies)",
        "modules (including needies)",
        "needy modules",
        "modules with their name containing \"Role Reversal\""
    };

    /// <summary>
    /// Indexable array of edgework string names.
    /// </summary>
    internal static string[] Edgework
    {
        get { return _strings; }
        set { Edgework = value; }
    }

    private static readonly string[] _colors = new string[10]
    {
        "navy",
        "lapis",
        "blue",
        "sky",
        "teal",
        "plum",
        "violet",
        "purple",
        "magenta",
        "lavender"
    };

    /// <summary>
    /// Indexable array of all colors used.
    /// </summary>
    internal static string[] Colors
    {
        get { return _colors; }
        set { Colors = value; }
    }

    private static readonly string[] _groupedColors = new string[10]
    {
        "blue",
        "blue",
        "blue",
        "blue",
        "blue",
        "purple",
        "purple",
        "purple",
        "purple",
        "purple"
    };

    /// <summary>
    /// Indexable array of all colors in an estimated format.
    /// </summary>
    internal static string[] GroupedColors
    {
        get { return _groupedColors; }
        set { GroupedColors = value; }
    }

    private static readonly string[] _ordinals = new string[9]
    {
        "first",
        "second",
        "third",
        "4th",
        "5th",
        "6th",
        "7th",
        "8th",
        "9th"
    };

    /// <summary>
    /// Convert index to equivalent ordinal.
    /// </summary>
    internal static string[] Ordinals
    {
        get { return _ordinals; }
        set { Ordinals = value; }
    }

    private static readonly string[] _tuplets = new string[9]
    {
        "monuple",
        "couple",
        "triple",
        "quadruple",
        "quintuple",
        "sextuple",
        "septuple",
        "octuple",
        "nonuple"
    };

    /// <summary>
    /// Convert index to equivalent tuplet.
    /// </summary>
    internal static string[] Tuplets
    {
        get { return _tuplets; }
        set { Tuplets = value; }
    }

    /// <summary>
    /// Generates the tutorial based on a few parameters that dictate the rules.
    /// </summary>
    /// <param name="buttonOrder">The indexes used for Interact in reading order.</param>
    /// <param name="baseN">The initial base shown on the module.</param>
    /// <param name="left">Append 0's on the left if true, otherwise right.</param>
    /// <param name="leftmost">Take leftmost digits if true, otherwise right.</param>
    /// <returns>The formatted condition array for the tutorial.</returns>
    internal Condition[] GetTutorial(List<int> buttonOrder, int baseN, ref bool left, ref bool leftmost, ref int offset)
    {
        string[] buttonText = { "left", "down", "up", "right" };

        return new Condition[]
        {
            new Condition { Text = "Welcome to Reformed Role Reversal! Press the " + buttonText[buttonOrder.IndexOf(2)] + " arrow button to advance." },
            new Condition { Text = "Convert the seed from Base-" + baseN + " to Base-10. Add 0's to the " + (left ? "left" : "right") + " of the number until you have 9 digits." },
            new Condition { Text = "Take the seed modulo 7 and add 3. The result is the amount of wires this module has." },
            new Condition { Text = "Take the " + (leftmost ? "leftmost" : "rightmost") + " digits matching the number of wires. With table " + offset + ", convert digits to colors to get the final wires." },
            new Condition { Text = "Jump to the set of conditions with the amount of wires with the bottom screen and press " + buttonText[buttonOrder.IndexOf(2)] + " if the condition is false." },
            new Condition { Text = "Once the first condition that applies has been discovered, enter submission mode by pressing either the " + buttonText[buttonOrder.IndexOf(0)] + " or " + buttonText[buttonOrder.IndexOf(3)] + " arrow button." },
            new Condition { Text = "NOTE: If a condition is true, but the wire to cut doesn't exist, skip the condition instead." },
            new Condition { Text = "Good luck!~ (" + _version + ")" }
        };
    }

    /// <summary>
    /// Gets the edgework from the same index as the _strings variable.
    /// </summary>
    /// <param name="i">The index for the _numbers array.</param>
    /// <returns>A number representing the edgework.</returns>
    internal int GetNumbers(int i)
    {
        return new int[17]
        {
            _info.GetBatteryCount(),
            _info.GetBatteryCount(Battery.AA) + _info.GetBatteryCount(Battery.AAx3) + _info.GetBatteryCount(Battery.AAx4),
            _info.GetBatteryCount(Battery.D),
            _info.GetBatteryHolderCount(),
            _info.GetIndicators().Count(),
            _info.GetOnIndicators().Count(),
            _info.GetOffIndicators().Count(),
            _info.GetPortPlateCount(),
            _info.GetPorts().Distinct().Count(),
            _info.GetPorts().Count() - _info.GetPorts().Distinct().Count(),
            _info.GetPortCount(),
            _info.GetSerialNumberNumbers().Count(),
            _info.GetSerialNumberLetters().Count(),
            _info.GetSolvableModuleNames().Count(),
            _info.GetModuleNames().Count(),
            _info.GetModuleNames().Count() - _info.GetSolvableModuleNames().Count(),
            _info.GetModuleNames().Count(s => s == "Role Reversal" || s == "Reformed Role Reversal")
        }[i];
    }
}
