using KModkit;
using System;
using System.Collections.Generic;
using System.Linq;

sealed class Manual
{
    private static readonly Random Rnd = new Random();

    #region Conditional Statements
    public static Condition First(int[] wires, KMBombInfo Info)
    {
        Edgework edgework = new Edgework(Info);
        int[] parameters = Randomize(3, 0, Edgework.Strings.Length);
        bool inversion = Rnd.NextDouble() > 0.5;

        parameters[0] = (parameters[0] / 5) + 2;
        parameters[2] = (parameters[2] % 2) + 3;

        Condition condition = new Condition
        {
            Text = inversion
            ? string.Format("If there's at most {0} {1}, skip to condition {2}.", parameters[0], Edgework.Strings[parameters[1]], parameters[2])
            : string.Format("If there's at least {0} {1}, skip to condition {2}.", parameters[0], Edgework.Strings[parameters[1]], parameters[2])
        };

        if ((!inversion && parameters[0] <= edgework.GetNumbers(parameters[1])) || (inversion && parameters[0] >= edgework.GetNumbers(parameters[1])))
            condition.Skip = parameters[2];

        return condition;
    }

    public static Condition A(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Randomize(2, 0, 10);
        Condition condition = new Condition
        {
            Text = string.Format("If a {0} wire is right of a {1} wire, cut the first {0} wire.", _colors[parameters[0]], _colors[parameters[1]])
        };

        for (int i = 1; i < wires.Length; i++)
            if (wires[i] == parameters[0] && wires[i - 1] == parameters[1])
            {
                condition.Wire = Find("firstInstanceOfKey", ref parameters[0], wires);
                break;
            }

        return condition;
    }

    public static Condition B(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Randomize(4, 0, 10);
        Condition condition = new Condition
        {
            Text = string.Format("If a {0} wire is left of a {1}, {2}, or {3} wire, cut the first {1}, {2}, or {3} wire.", _colors[parameters[0]], _colors[parameters[1]], _colors[parameters[2]], _colors[parameters[3]])
        };

        for (int i = 1; i < wires.Length; i++)
            if (wires[i - 1] == parameters[0] && (wires[i] == parameters[1] || wires[i] == parameters[2] || wires[i] == parameters[3]))
            {
                condition.Wire = EarliestIndex(new int[] { Find("firstInstanceOfKey", ref parameters[1], wires),
                                                           Find("firstInstanceOfKey", ref parameters[2], wires),
                                                           Find("firstInstanceOfKey", ref parameters[3], wires) });
                break;
            }

        return condition;
    }

    public static Condition C(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Randomize(3, 0, wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If the {0}, {1}, or {2} wire share any color, cut the first wire that isn't the shared color.", _ordinals[parameters[0]], _ordinals[parameters[1]], _ordinals[parameters[2]])
        };

        int matchingWire = wires[parameters[0]] == wires[parameters[1]] ? wires[parameters[0]] : wires[parameters[2]];

        if (wires[parameters[0]] == wires[parameters[1]] || wires[parameters[1]] == wires[parameters[2]] || wires[parameters[2]] == wires[parameters[0]])
            condition.Wire = Find("firstInstanceOfNotKey", ref matchingWire, wires);

        return condition;
    }

    public static Condition D(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Randomize(1, (wires.Length / 2) + 1, wires.Length - 1);
        Condition condition = new Condition
        {
            Text = string.Format("If there are {0} wires with matching colors, cut the last wire that isn't the matching color.", parameters[0])
        };

        int[] colors = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < wires.Length; i++)
            colors[wires[i]]++;

        for (int i = 0; i < colors.Length; i++)
            if (colors[i] >= (wires.Length / 2) + 1)
            {
                condition.Wire = Find("lastInstanceOfNotKey", ref i, wires);
                break;
            }

        return condition;
    }

    public static Condition E(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Randomize(1, (wires.Length / 2) + 1, wires.Length - 1);
        Condition condition = new Condition
        {
            Text = string.Format("If there are {0} wires with matching colors, cut the last wire that isn't the matching color.", parameters[0])
        };

        int[] colors = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < wires.Length; i++)
            colors[wires[i]]++;

        for (int i = 0; i < colors.Length; i++)
            if (colors[i] >= (wires.Length / 2) + 1)
            {
                condition.Wire = Find("lastInstanceOfNotKey", ref i, wires);
                break;
            }

        return condition;
    }

    public static Condition W(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Randomize(2, 0, 10);
        Condition condition = new Condition
        {
            Text = string.Format("If there are less batteries than {0} wires, cut the last non-{1}ish wire.", _colors[parameters[0]], _generalColors[parameters[0]])
        };

        int matches = 0;

        for (int i = 0; i < wires.Length; i++)
        {
            if (wires[i] == parameters[0])
                matches++;
        }

        if (Info.GetBatteryCount() < matches)
            condition.Wire = parameters[1] < 5
                ? Find("lastInstanceOfPurple", ref parameters[0], wires)
                : Find("lastInstanceOfBlue", ref parameters[0], wires);

        return condition;
    }

    public static Condition X(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Randomize(1, 0, 10);
        Condition condition = new Condition
        {
            Text = string.Format("If there are less batteries than {0} wires, cut the first non-{0} wire.", _colors[parameters[0]])
        };

        int matches = 0;

        for (int i = 0; i < wires.Length; i++)
        {
            if (wires[i] == parameters[0])
                matches++;
        }

        if (Info.GetBatteryCount() < matches)
            condition.Wire = Find("firstInstanceOfNotKey", ref parameters[0], wires);

        return condition;
    }

    public static Condition Y(int[] wires, KMBombInfo Info)
    {
        Condition condition = new Condition
        {
            Text = string.Format("If the serial number has a digit matching any amount of wires present excluding 0, cut the last wire that is most common.")
        };

        int[] colors = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < wires.Length; i++)
            colors[wires[i]]++;

        for (int i = 0; i < colors.Length; i++)
            if (colors[i] != 0 && Info.GetSerialNumberNumbers().Contains(colors[i]))
                for (int j = 9; j >= 1; j--)
                {
                    List<int> potentialWires = new List<int>(0);

                    for (int k = 0; k < colors.Length; k++)
                        if (colors[k] == j)
                            potentialWires.Add(k);

                    for (int k = 0; k < potentialWires.Count; k++)
                    {
                        int check = potentialWires[k];
                        potentialWires[k] = Find("lastInstanceOfKey", ref check, wires);
                        condition.Wire = EarliestIndex(potentialWires);
                    }
                }

        return condition;
    }

    public static Condition Z(int[] wires, KMBombInfo Info)
    {
        Condition condition = new Condition
        {
            Text = string.Format("If the serial number has a digit matching the amount of the most common wires, cut the first wire that is least common.")
        };

        int[] colors = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        for (int i = 0; i < wires.Length; i++)
            colors[wires[i]]++;

        if (Info.GetSerialNumberNumbers().Contains(colors.Max()))
            for (int i = 1; i <= 9; i++)
            {
                List<int> potentialWires = new List<int>(0);
                for (int j = 0; j < colors.Length; j++)
                {
                    if (colors[j] == i)
                        potentialWires.Add(j);
                        
                }

                for (int j = 0; j < potentialWires.Count; j++)
                {
                    int check = potentialWires[j];
                    potentialWires[j] = Find("firstInstanceOfKey", ref check, wires);
                    condition.Wire = EarliestIndex(potentialWires);
                }
            }

        return condition;
    }

    public static Condition Last(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Randomize(1, 0, wires.Length - 1);
        Condition condition = new Condition
        {
            Text = string.Format("Otherwise, cut the {0} wire.", _ordinals[parameters[0]])
        };

        condition.Wire = ++parameters[0];

        return condition;
    }
    #endregion

    #region Algorithms
    public static int Find(string method, ref int key, int[] wires)
    {
        switch (method)
        {
            case "firstInstanceOfKey":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] == key)
                        return i + 1;
                break;

            case "firstInstanceOfNotKey":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] != key)
                        return i + 1;
                break;

            case "lastInstanceOfKey":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] == key)
                        return i + 1;
                break;

            case "lastInstanceOfNotKey":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] != key)
                        return i + 1;
                break;

            case "firstInstanceOfBlue":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] < 5)
                        return i + 1;
                break;

            case "firstInstanceOfPurple":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] >= 5)
                        return i + 1;
                break;

            case "lastInstanceOfBlue":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] < 5)
                        return i + 1;
                break;

            case "lastInstanceOfPurple":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] >= 5)
                        return i + 1;
                break;

            default:
                throw new NotImplementedException();
        }
        
        return 0;
    }

    public static int EarliestIndex(int[] array)
    {
        int min = 10;

        for (int i = 0; i < array.Length; i++)
            if (min > array[i] && array[i] != 0)
                min = array[i];

        return min;
    }

    private static int EarliestIndex(List<int> array)
    {
        int min = 10;

        for (int i = 0; i < array.Count; i++)
            if (min > array[i] && array[i] != 0)
                min = array[i];

        return min;
    }

    public static int[] Randomize(int length, int minValue, int maxValue)
    {
        int[] parameters = new int[length];
        for (int i = 0; i < length; i += 0)
        {
            int random = Rnd.Next(minValue, maxValue);

            if (!parameters.Contains(random))
            {
                parameters[i] = random;
                i++;
            }
        }
        return parameters;
    }
    #endregion

    #region String Arrays
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
    #endregion
}