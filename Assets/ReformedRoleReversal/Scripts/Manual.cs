using KModkit;
using System;
using System.Collections.Generic;
using System.Linq;

sealed class Manual
{
    private static readonly Random Rnd = new Random();
    
    public static Condition FirstA(int[] wires, KMBombInfo Info)
    {
        StaticArrays edgework = new StaticArrays(Info);
        int[] parameters = Algorithms.Randomize(arrayLength: 3, minValue: 0, maxValue: StaticArrays.Strings.Length);
        bool inversion = Rnd.NextDouble() > 0.5;

        parameters[0] = (parameters[0] / 5) + 2;
        parameters[2] = (parameters[2] % 2) + 3;

        Condition condition = new Condition
        {
            Text = inversion
                 ? string.Format("If there's at most {0} {1}, skip to condition {2}.", parameters[0], StaticArrays.Strings[parameters[1]], parameters[2])
                 : string.Format("If there's at least {0} {1}, skip to condition {2}.", parameters[0], StaticArrays.Strings[parameters[1]], parameters[2])
        };

        if ((!inversion && parameters[0] <= edgework.GetNumbers(parameters[1])) || (inversion && parameters[0] >= edgework.GetNumbers(parameters[1])))
            condition.SkipTo = parameters[2];

        return condition;
    }

    public static Condition FirstB(int[] wires, KMBombInfo Info)
    {
        StaticArrays edgework = new StaticArrays(Info);
        int[] parameters = Algorithms.Randomize(arrayLength: 3, minValue: 0, maxValue: StaticArrays.Strings.Length);
        bool moreThan = Rnd.NextDouble() > 0.5;

        parameters[2] = (parameters[2] % 2) + 3;

        Condition condition = new Condition
        {
            Text = moreThan
                 ? string.Format("If there's more {0} than {1}, skip to condition {2}.", StaticArrays.Strings[parameters[0]], StaticArrays.Strings[parameters[1]], parameters[2])
                 : string.Format("If there's less {0} than {1}, skip to condition {2}.", StaticArrays.Strings[parameters[0]], StaticArrays.Strings[parameters[1]], parameters[2])
        };

        if ((!moreThan && edgework.GetNumbers(parameters[0]) < edgework.GetNumbers(parameters[1])) || (moreThan && edgework.GetNumbers(parameters[0]) > edgework.GetNumbers(parameters[1])))
            condition.SkipTo = parameters[2];

        return condition;
    }

    public static Condition A(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 2, minValue: 0, maxValue: 10);
        Condition condition = new Condition
        {
            Text = string.Format("If a {0} wire is right of a {1} wire, cut the first {0} wire.", StaticArrays.Colors[parameters[0]], StaticArrays.Colors[parameters[1]])
        };

        for (int i = 1; i < wires.Length; i++)
            if (wires[i] == parameters[0] && wires[i - 1] == parameters[1])
            {
                condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref parameters[0], wires: wires);
                break;
            }

        return condition;
    }

    public static Condition B(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 4, minValue: 0, maxValue: 10);
        Condition condition = new Condition
        {
            Text = string.Format("If a {0} wire is left of a {1}, {2}, or {3} wire, cut the first {1}, {2}, or {3} wire.", StaticArrays.Colors[parameters[0]], StaticArrays.Colors[parameters[1]], StaticArrays.Colors[parameters[2]], StaticArrays.Colors[parameters[3]])
        };

        for (int i = 1; i < wires.Length; i++)
            if (wires[i - 1] == parameters[0] && (wires[i] == parameters[1] || wires[i] == parameters[2] || wires[i] == parameters[3]))
            {
                condition.Wire = Algorithms.EarliestIndex(new int[] { Algorithms.Find(method: "firstInstanceOfKey", key: ref parameters[1], wires: wires),
                                                                      Algorithms.Find(method: "firstInstanceOfKey", key: ref parameters[2], wires: wires),
                                                                      Algorithms.Find(method: "firstInstanceOfKey", key: ref parameters[3], wires: wires) });
                break;
            }

        return condition;
    }

    public static Condition C(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 3, minValue: 0, maxValue: wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If the {0}, {1}, or {2} wire share any color, cut the first wire that isn't the shared color.", StaticArrays.Ordinals[parameters[0]], StaticArrays.Ordinals[parameters[1]], StaticArrays.Ordinals[parameters[2]])
        };

        int matchingWire = wires[parameters[0]] == wires[parameters[1]] ? wires[parameters[0]] : wires[parameters[2]];

        if (wires[parameters[0]] == wires[parameters[1]] || wires[parameters[1]] == wires[parameters[2]] || wires[parameters[2]] == wires[parameters[0]])
            condition.Wire = Algorithms.Find(method: "firstInstanceOfNotKey", key: ref matchingWire, wires: wires);

        return condition;
    }

    public static Condition D(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 1, minValue: (wires.Length / 2) + 1, maxValue: wires.Length - 1);
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
                condition.Wire = Algorithms.Find(method: "lastInstanceOfNotKey", key: ref i, wires: wires);
                break;
            }

        return condition;
    }

    public static Condition E(int[] wires, KMBombInfo Info)
    {
        int highestValue = wires.Max();
        Condition condition = new Condition
        {
            Text = string.Format("If all wires are unique, cut the wire with the highest value.")
        };

        if (wires.Distinct().Count() == wires.Count())
            condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref highestValue, wires: wires);

        return condition;
    }

    public static Condition F(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 1, minValue: 1, maxValue: wires.Length - 1);
        int lowestWire = wires.Min();
        bool seenNotUnique = false;

        Condition condition = new Condition
        {
            Text = string.Format("If all wires are unique except for 1 {0} with matching colors, cut the first wire with the lowest value.", StaticArrays.Tuplets[parameters[0]])
        };

        foreach (IGrouping<int, int> number in wires.GroupBy(x => x))
        {
            if (number.Count() == 1)
                continue;
            
            if (number.Count() == parameters[0] + 1 && !seenNotUnique)
            {
                seenNotUnique = true;
                continue;
            }

            goto conditionFalse;
        }

        if (seenNotUnique)
            condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref lowestWire, wires: wires);

    conditionFalse:

        return condition;
    }

    public static Condition G(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 1, minValue: 1, maxValue: (wires.Length / 2) + 1);
        Array.Sort(wires);

        int exceptions = 0,
            middleWire1 = wires[wires.Length / 2],
            middleWire2 = wires.Length % 2 == 0
                        ? wires[(int)(((float)wires.Length / 2) + 0.6f)]
                        : middleWire1;

        Condition condition = new Condition
        {
            Text = string.Format("If all wires aren't unique excluding {0}, cut the last wire with the median(s).", parameters[0])
        };

        foreach (IGrouping<int, int> number in wires.GroupBy(x => x))
        {
            if (number.Count() != 1)
                continue;

            exceptions++;

            if (exceptions > parameters[0])
                goto conditionFalse;
        }

        condition.Wire = Math.Max(Algorithms.Find(method: "lastInstanceOfKey", key: ref middleWire1, wires: wires),
                                  Algorithms.Find(method: "lastInstanceOfKey", key: ref middleWire2, wires: wires));

    conditionFalse:

        return condition;
    }

    public static Condition V(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 2, minValue: 0, maxValue: 10);
        Condition condition = new Condition
        {
            Text = string.Format("If there are less batteries than {0} wires, cut the last non-{1}ish wire.", StaticArrays.Colors[parameters[0]], StaticArrays.GeneralColors[parameters[0]])
        };

        int matches = 0;

        for (int i = 0; i < wires.Length; i++)
            if (wires[i] == parameters[0])
                matches++;

        if (Info.GetBatteryCount() < matches)
            condition.Wire = parameters[1] < 5
                ? Algorithms.Find(method: "lastInstanceOfPurple", key: ref parameters[0], wires: wires)
                : Algorithms.Find(method: "lastInstanceOfBlue", key: ref parameters[0], wires: wires);

        return condition;
    }

    public static Condition W(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 2, minValue: 0, maxValue: 10);
        Condition condition = new Condition
        {
            Text = string.Format("If there are less batteries than {0} wires, cut the last non-{1}ish wire.", StaticArrays.Colors[parameters[0]], StaticArrays.GeneralColors[parameters[0]])
        };

        int matches = 0;

        for (int i = 0; i < wires.Length; i++)
            if (wires[i] == parameters[0])
                matches++;

        if (Info.GetBatteryCount() < matches)
            condition.Wire = parameters[1] < 5
                ? Algorithms.Find(method: "lastInstanceOfPurple", key: ref parameters[0], wires: wires)
                : Algorithms.Find(method: "lastInstanceOfBlue", key: ref parameters[0], wires: wires);

        return condition;
    }

    public static Condition X(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 1, minValue: 0, maxValue: 10);
        Condition condition = new Condition
        {
            Text = string.Format("If there are less batteries than {0} wires, cut the first non-{0} wire.", StaticArrays.Colors[parameters[0]])
        };

        int matches = 0;

        for (int i = 0; i < wires.Length; i++)
            if (wires[i] == parameters[0])
                matches++;

        if (Info.GetBatteryCount() < matches)
            condition.Wire = Algorithms.Find(method: "firstInstanceOfNotKey", key: ref parameters[0], wires: wires);

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
                        potentialWires[k] = Algorithms.Find(method: "lastInstanceOfKey", key: ref check, wires: wires);
                        condition.Wire = Algorithms.EarliestIndex(potentialWires);
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
                    if (colors[j] == i)
                        potentialWires.Add(j);

                for (int j = 0; j < potentialWires.Count; j++)
                {
                    int check = potentialWires[j];
                    potentialWires[j] = Algorithms.Find(method: "firstInstanceOfKey", key: ref check, wires: wires);
                    condition.Wire = Algorithms.EarliestIndex(potentialWires);
                }
            }

        return condition;
    }

    public static Condition LastA(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 1, minValue: 0, maxValue: wires.Length - 1);
        Condition condition = new Condition
        {
            Text = string.Format("Otherwise, cut the {0} wire.", StaticArrays.Ordinals[parameters[0]])
        };

        condition.Wire = ++parameters[0];

        return condition;
    }

    public static Condition LastB(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Randomize(arrayLength: 1, minValue: 0, maxValue: wires.Length - 1);
        parameters[0] = wires[parameters[0]];

        Condition condition = new Condition
        {
            Text = string.Format("Otherwise, cut the first {0} wire.", StaticArrays.Colors[parameters[0]])
        };

        condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref parameters[0], wires: wires);

        return condition;
    }
}
