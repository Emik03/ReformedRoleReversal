﻿using KModkit;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains mostly static unchanging information and indexable edgework.
/// </summary>
internal class Arrays
{
    internal Arrays(KMBombInfo Info)
    {
        info = Info;
    }

    private readonly KMBombInfo info;

    /// <summary>
    /// The version of the module.
    /// </summary>
    internal static string Version
    {
        get { return version; }
        set { Version = value; }
    }

    // When updating, change this string!
    private const string version = "v0.9b";

    /// <summary>
    /// Indexable array of indicator edgework in alphabetical order.
    /// </summary>
    internal static Indicator[] Indicators
    {
        get { return indicators; }
        set { Indicators = value; }
    }

    private static readonly Indicator[] indicators = new Indicator[11]
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
    /// Indexable array of indicator string names in alphabetical order.
    /// </summary>
    internal static string[] IndicatorNames
    {
        get { return indicatorNames; }
        set { IndicatorNames = value; }
    }

    private static readonly string[] indicatorNames = new string[11]
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
    /// 62 length array consisting of 0-9, A-Z, a-z
    /// </summary>
    internal static char[] Base62
    {
        get { return base62; }
        set { Base62 = value; }
    }

    private static readonly char[] base62 = new char[62]
    {
        '0','1','2','3','4','5','6','7','8','9',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z' 
    };

    /// <summary>
    /// Indexable array of edgework string names.
    /// </summary>
    internal static string[] Edgework
    {
        get { return edgework; }
        set { Edgework = value; }
    }

    private static readonly string[] edgework = new string[20]
    {
        "batteries",
        "AA batteries",
        "D batteries",
        "battery holders",
        "indicators",
        "lit indicators",
        "unlit indicators",
        "indicators sharing a letter in \"Role\"",
        "port plates",
        "unique ports",
        "duplicate ports",
        "ports",
        "numbers in the serial number",
        "letters in the serial number",
        "vowels in the serial number",
        "consonants in the serial number",
        "total modules (excluding needies)",
        "total modules (including needies)",
        "needy modules",
        "modules with their name containing \"Role Reversal\""
    };

    /// <summary>
    /// Indexable array of all colors used.
    /// </summary>
    internal static string[] Colors
    {
        get { return colors; }
        set { Colors = value; }
    }

    private static readonly string[] colors = new string[10]
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
    /// Indexable array of all colors in an estimated format.
    /// </summary>
    internal static string[] GroupedColors
    {
        get { return groupedColors; }
        set { GroupedColors = value; }
    }

    private static readonly string[] groupedColors = new string[10]
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
    /// Convert index to equivalent ordinal.
    /// </summary>
    internal static string[] Ordinals
    {
        get { return ordinals; }
        set { Ordinals = value; }
    }

    private static readonly string[] ordinals = new string[9]
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
    /// Convert index to equivalent tuplet.
    /// </summary>
    internal static string[] Tuplets
    {
        get { return tuplets; }
        set { Tuplets = value; }
    }

    private static readonly string[] tuplets = new string[9]
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
    /// Generates the tutorial based on a few parameters that dictate the rules.
    /// </summary>
    /// <param name="buttonOrder">The indexes used for Interact in reading order.</param>
    /// <param name="baseN">The initial base shown on the module.</param>
    /// <param name="left">Append 0's on the left if true, otherwise right.</param>
    /// <param name="mod">The number to modulo the seed with.</param>
    /// <param name="add">The number to add after modulo.</param>
    /// <param name="leftmost">Take leftmost digits if true, otherwise right.</param>
    /// <param name="offset">The offset to apply for the table.</param>
    /// <param name="discard">Whether or not to discard wires.</param>
    /// <param name="append">Which direction to append wires.</param>
    /// <returns>The formatted condition array for the tutorial.</returns>
    internal Condition[] GetTutorial(List<int> buttonOrder, int baseN, ref bool left, ref int mod, ref int add, ref bool leftmost, ref int offset, ref bool discard, ref bool append)
    {
        string[] buttonText = { "left", "down", "up", "right" };

        return new Condition[]
        {
            new Condition { Text = "Welcome to Reformed Role Reversal! Press the " + buttonText[buttonOrder.IndexOf(2)] + " arrow button to advance." },
            new Condition { Text = "Look around the screen and locate the seed. Convert it from Base-" + baseN + " to Base-10. Add 0's to the " + (left ? "left" : "right") + " of this number until you have 9 digits." },
            new Condition { Text = "Take the seed modulo " + mod + " and add " + add + ". The result is the amount of wires this module has." },
            new Condition { Text = "Take the " + (leftmost ? "leftmost" : "rightmost") + " digits matching the number of wires. With lookup #" + offset + ", convert digits to colors to get the final wires." },
            new Condition { Text = "Jump to the set of conditions with the amount of wires with the bottom screen and press " + buttonText[buttonOrder.IndexOf(2)] + " if the condition is false." },
            new Condition { Text = "When told to " + (discard ? "discard" : "append") + " wires, " + (discard ? string.Empty : "add the wires to the " + (append ? "left, then" : "right, then")) + " refer to the set of conditions with the new amount of wires." },
            new Condition { Text = "Once the first condition that applies has been discovered, enter submission mode by pressing either the " + buttonText[buttonOrder.IndexOf(0)] + " or " + buttonText[buttonOrder.IndexOf(3)] + " arrow button." },
            new Condition { Text = "NOTE: If the condition specifies to cut a nonexistent wire, skip it instead. Good luck!~ (" + version + ")" }
        };
    }

    /// <summary>
    /// Gets the edgework from the same index as the strings variable.
    /// </summary>
    /// <param name="i">The index for the numbers array.</param>
    /// <returns>A number representing the edgework.</returns>
    internal int GetNumbers(int i)
    {
        return new int[20]
        {
            info.GetBatteryCount(),
            info.GetBatteryCount(Battery.AA) + info.GetBatteryCount(Battery.AAx3) + info.GetBatteryCount(Battery.AAx4),
            info.GetBatteryCount(Battery.D),
            info.GetBatteryHolderCount(),
            info.GetIndicators().Count(),
            info.GetOnIndicators().Count(),
            info.GetOffIndicators().Count(),
            info.GetIndicators().Where(x => x.Any(y => new[] { 'r', 'o', 'l', 'e' }.Contains(y))).Count(),
            info.GetPortPlateCount(),
            info.GetPorts().Distinct().Count(),
            info.GetPorts().Count() - info.GetPorts().Distinct().Count(),
            info.GetPortCount(),
            info.GetSerialNumberNumbers().Count(),
            info.GetSerialNumberLetters().Count(),
            info.GetSerialNumberLetters().Where(x => new[] { 'a', 'e', 'i', 'o', 'u' }.Contains(x)).Count(),
            info.GetSerialNumberLetters().Where(x => !new[] { 'a', 'e', 'i', 'o', 'u', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }.Contains(x)).Count(),
            info.GetSolvableModuleNames().Count(),
            info.GetModuleNames().Count(),
            info.GetModuleNames().Count() - info.GetSolvableModuleNames().Count(),
            info.GetModuleNames().Count(s => s == "Role Reversal" || s == "Reformed Role Reversal")
        }[i];
    }
}
