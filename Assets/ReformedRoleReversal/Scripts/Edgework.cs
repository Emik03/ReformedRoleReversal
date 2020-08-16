using KModkit;
using System.Linq;

internal class Edgework
{
    internal Edgework(KMBombInfo Info)
    {
        _info = Info;
    }

    private KMBombInfo _info;

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
        "unique port plates",
        "duplicate port plates",
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
}
