using KModkit;
using System.Linq;

internal class StaticArrays
{
    internal StaticArrays(KMBombInfo Info)
    {
        _info = Info;
    }

    private readonly KMBombInfo _info;

    private static readonly Condition[] _tutorial =
    {
        new Condition { Text = "Welcome to Reformed Role Reversal! Press the up arrow button to continue." },
        new Condition { Text = "Convert the seed from Base-62 to Base-10. Add 0's to the left of the number until you have 10 digits." },
        new Condition { Text = "Take the seed modulo 7. The result is the amount of wires this module has." },
        new Condition { Text = "Take the leftmost digits matching the number of wires, and convert the digits to colors to obtain the final wires." },
        new Condition { Text = "Jump to the next set of conditions with the bottom screen, moving onto the next condition if it's false." },
        new Condition { Text = "Select with the left and right arrow buttons and hold the screen to cut the wire." },
        new Condition { Text = "NOTE: If a condition is true, but the wire to cut doesn't exist, skip the condition instead." },
    };

    internal static Condition[] Tutorial
    {
        get { return _tutorial; }
        set { Tutorial = value; }
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

    internal static string[] Strings
    {
        get { return _strings; }
        set { Strings = value; }
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

    internal static string[] Colors
    {
        get { return _colors; }
        set { Colors = value; }
    }

    private static readonly string[] _generalColors = new string[10]
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

    internal static string[] GeneralColors
    {
        get { return _generalColors; }
        set { GeneralColors = value; }
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

    internal static string[] Tuplets
    {
        get { return _tuplets; }
        set { Tuplets = value; }
    }

    /// <summary>
    /// Gets the edgework from the same index as the _strings variable.
    /// </summary>
    /// <param name="i">The index for the _numbers array.</param>
    /// <returns>A number representing the edgework.</returns>
    internal int GetNumbers(int i)
    {
        int[] _numbers = new int[17]
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
        };

        return _numbers[i];
    }
}
