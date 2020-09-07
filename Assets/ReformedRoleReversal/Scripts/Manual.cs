﻿using KModkit;
using System;
using System.Collections.Generic;
using System.Linq;

static class Manual
{
    private static readonly Random rnd = new Random();

    #region First/Second Conditions
    public static Condition FirstA(int[] wires, int lookup, KMBombInfo Info, bool firstCondition)
    {
        int[] parameters = Algorithms.Random(length: 3, min: 0, max: Arrays.Edgework.Length);
        int edgework = new Arrays(Info).GetNumbers(parameters[1]);
        bool inversion = rnd.NextDouble() > 0.5, leftmost = rnd.NextDouble() > 0.5;

        parameters[0] = (parameters[0] / 5) + 2;
        parameters[2] = (parameters[2] % 2) + 3;

        Condition condition = new Condition
        {
             Text = firstCondition ? string.Format("If there's at {0} {1} {2}, skip to condition {3}.", inversion ? "most" : "least", parameters[0], Arrays.Edgework[parameters[1]], parameters[2])
                                   : string.Format("If there's at {0} {1} {2}, remove the {3}{4} wire{5}.", inversion ? "most" : "least", parameters[0], Arrays.Edgework[parameters[1]], Math.Abs(parameters[2]) - 2 != 1 ? Math.Abs(parameters[2] - 2).ToString() + ' ' : string.Empty, leftmost ? "leftmost" : "rightmost", Math.Abs(parameters[2]) - 2 != 1 ? "s" : string.Empty)
        };

        if ((!inversion && parameters[0] <= edgework) || (inversion && parameters[0] >= edgework))
            if (firstCondition)
                condition.Skip = parameters[2];
            else
                condition.Remove = leftmost ? -(parameters[2] - 2) : parameters[2] - 2;

        return condition;
    }

    public static Condition FirstB(int[] wires, int lookup, KMBombInfo Info, bool firstCondition)
    {
        Arrays staticArrays = new Arrays(Info);

        int[] parameters = Algorithms.Random(length: 3, min: 0, max: Arrays.Edgework.Length);
        int edgework1 = staticArrays.GetNumbers(parameters[0]), edgework2 = staticArrays.GetNumbers(parameters[1]);
        bool more = rnd.NextDouble() > 0.5, leftmost = rnd.NextDouble() > 0.5;

        parameters[2] = (parameters[2] % 2) + 3;

        Condition condition = new Condition
        {
            Text = firstCondition ? string.Format("If there's {0} {1} than {2}, skip to condition {3}.", more ? "more" : "less", Arrays.Edgework[parameters[0]], Arrays.Edgework[parameters[1]], parameters[2])
                                  : string.Format("If there's {0} {1} than {2}, remove the {3}{4} wire{5}.", more ? "more" : "less", Arrays.Edgework[parameters[0]], Arrays.Edgework[parameters[1]], Math.Abs(parameters[2]) - 2 != 1 ? Math.Abs(parameters[2] - 2).ToString() + ' ' : string.Empty, leftmost ? "leftmost" : "rightmost", Math.Abs(parameters[2]) - 2 != 1 ? "s" : string.Empty)
        };

        if ((!more && edgework1 < edgework2) || (more && edgework1 > edgework2))
            if (firstCondition)
                condition.Skip = parameters[2];
            else
                condition.Remove = leftmost ? -(parameters[2] - 2) : parameters[2] - 2;

        return condition;
    }

    public static Condition FirstC(int[] wires, int lookup, KMBombInfo Info, bool firstCondition)
    {
        Arrays staticArrays = new Arrays(Info);

        int[] parameters = Algorithms.Random(length: 3, min: 0, max: Arrays.Edgework.Length);
        int edgework1 = staticArrays.GetNumbers(parameters[0]), edgework2 = staticArrays.GetNumbers(parameters[1]);
        bool orAnd = rnd.NextDouble() > 0.5, inversion = rnd.NextDouble() > 0.5, leftmost = rnd.NextDouble() > 0.5;

        parameters[2] = (parameters[2] % 2) + 3;

        Condition condition = new Condition
        {
            Text = firstCondition ? string.Format("If {0} {1} {2} {3} exist, skip to condition {4}.", Arrays.Edgework[parameters[0]], orAnd ? "or" : "and", Arrays.Edgework[parameters[1]], inversion ? "don't" : "do", parameters[2])
                                  : string.Format("If {0} {1} {2} {3} exist, remove the {4}{5} wire{6}.", Arrays.Edgework[parameters[0]], orAnd ? "or" : "and", Arrays.Edgework[parameters[1]], inversion ? "don't" : "do", Math.Abs(parameters[2]) - 2 != 1 ? Math.Abs(parameters[2] - 2).ToString() + ' ' : string.Empty, leftmost ? "leftmost" : "rightmost", Math.Abs(parameters[2]) - 2 != 1 ? "s" : string.Empty)
        };

        if (inversion && ((orAnd && (edgework1 == 0 || edgework2 == 0)) || (!orAnd && edgework1 == 0 && edgework2 == 0))
        || !inversion && ((orAnd && (edgework1 != 0 || edgework2 != 0)) || (!orAnd && edgework1 != 0 && edgework2 != 0)))
            if (firstCondition)
                condition.Skip = parameters[2];
            else
                condition.Remove = leftmost ? -(parameters[2] - 2) : parameters[2] - 2;

        return condition;
    }
    #endregion

    #region Noninitial Conditions
    public static Condition A(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: Arrays.Colors.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If a {0} wire is right of a {1} wire, cut the first {0} wire.", Arrays.Colors[parameters[0]], Arrays.Colors[parameters[1]])
        };

        for (int i = 1; i < wires.Length; i++)
            if (wires[i] == parameters[0] && wires[i - 1] == parameters[1])
            {
                condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref parameters[0], wires: wires);
                break;
            }

        return condition;
    }

    public static Condition B(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 4, min: 0, max: Arrays.Colors.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If a {0} wire is left of a {1}, {2}, or {3} wire, cut the first {1}, {2}, or {3} wire.", Arrays.Colors[parameters[0]], Arrays.Colors[parameters[1]], Arrays.Colors[parameters[2]], Arrays.Colors[parameters[3]])
        };

        for (int i = 1; i < wires.Length; i++)
            if (wires[i - 1] == parameters[0] && (wires[i] == parameters[1] || wires[i] == parameters[2] || wires[i] == parameters[3]))
            {
                condition.Wire = Algorithms.First(new int?[] { Algorithms.Find(method: "firstInstanceOfKey", key: ref parameters[1], wires: wires),
                                                               Algorithms.Find(method: "firstInstanceOfKey", key: ref parameters[2], wires: wires),
                                                               Algorithms.Find(method: "firstInstanceOfKey", key: ref parameters[3], wires: wires) });
                break;
            }

        return condition;
    }

    public static Condition C(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 3, min: 0, max: wires.Length + 1);
        Condition condition = new Condition
        {
            Text = string.Format("If the {0}, {1}, or {2} wire share any color, cut the first wire that isn't the shared color.", Arrays.Ordinals[parameters[0]], Arrays.Ordinals[parameters[1]], Arrays.Ordinals[parameters[2]])
        };
        
        if (wires[parameters[0]] == wires[parameters[1]] ||
            wires[parameters[1]] == wires[parameters[2]] ||
            wires[parameters[2]] == wires[parameters[0]])
        {
            int matchingWire = wires[parameters[0]] == wires[parameters[1]] ? wires[parameters[0]] : wires[parameters[2]];
            condition.Wire = Algorithms.Find(method: "firstInstanceOfNotKey", key: ref matchingWire, wires: wires);
        }

        return condition;
    }

    public static Condition D(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next((int)Math.Ceiling((float)wires.Length / 2), wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If there are {0} wires with matching colors, cut the last wire that isn't the matching color.", parameter)
        };

        int[] colors = Algorithms.GetColors(grouped: false, wires: wires);
        
        for (int i = 0; i < colors.Length; i++)
            if (colors[i] >= parameter)
            {
                condition.Wire = Algorithms.Find(method: "lastInstanceOfNotKey", key: ref i, wires: wires);
                break;
            }

        return condition;
    }

    public static Condition E(int[] wires, int lookup, KMBombInfo Info)
    {
        bool highest = rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("If all wires are unique, cut the wire with the {0} value.", highest ? "highest" : "lowest")
        };

        if (wires.Distinct().Count() == wires.Count())
        {
            int[] revertedWires = Algorithms.RevertLookup(wires, ref lookup);
            int key = highest ? revertedWires.Max() : revertedWires.Min();

            condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref key, wires: revertedWires);
        }

        return condition;
    }

    public static Condition F(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(1, wires.Length - 1);
        bool seenException = false, lowest = rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("If all wires are unique except for 1 {0} with matching colors, cut the first wire with the {1} value.", Arrays.Tuplets[parameter], lowest ? "lowest" : "highest")
        };

        foreach (IGrouping<int, int> number in wires.GroupBy(x => x))
        {
            if (number.Count() == 1)
                continue;
            
            if (seenException || number.Count() != parameter + 1)
                return condition;

            seenException = true;
        }

        int[] revertedWires = Algorithms.RevertLookup(wires, ref lookup);
        int key = lowest ? revertedWires.Min() : revertedWires.Max();

        condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref key, wires: revertedWires);

        return condition;
    }

    public static Condition G(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(1, (wires.Length / 2) + 1);
        Array.Sort(wires);

        int exceptions = 0,
            middleWire1 = wires[wires.Length / 2],
            middleWire2 = wires.Length % 2 == 0
                        ? wires[(int)(((float)wires.Length / 2) + 0.6f)]
                        : middleWire1;

        Condition condition = new Condition
        {
            Text = string.Format("If all wires aren't unique excluding {0}, cut the last wire whose value {1} the median{2}.", parameter, wires.Length % 2 == 1 ? "is" : "are", wires.Length % 2 == 1 ? string.Empty : "s")
        };

        foreach (IGrouping<int, int> number in wires.GroupBy(x => x))
            if (number.Count() != 1)
                exceptions++;

        if (exceptions != parameter)
            return condition;

        condition.Wire = Math.Max((int)Algorithms.Find(method: "lastInstanceOfKey", key: ref middleWire1, wires: wires),
                                  (int)Algorithms.Find(method: "lastInstanceOfKey", key: ref middleWire2, wires: wires));

        if (condition.Wire == 0)
            condition.Wire = null;

        return condition;
    }

    public static Condition H(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: Arrays.Colors.Length);
        bool inversion = rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("If there {0} any {1} wires, cut the first {2}ish wire.", inversion ? "aren't" : "are", Arrays.Colors[parameters[0]], Arrays.GroupedColors[parameters[1]])
        };

        string method = parameters[1] < 5 ? "firstInstanceOfPurple" : "firstInstanceOfBlue";

        if (inversion ^ wires.Contains(parameters[0]))
            condition.Wire = Algorithms.Find(method: method, key: ref parameters[0], wires: wires);

        return condition;
    }

    public static Condition I(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 3, min: 0, max: Arrays.GroupedColors.Length);
        parameters[0] = rnd.Next((int)Math.Ceiling((float)wires.Length / 2), wires.Length);

        Condition condition = new Condition
        {
            Text = string.Format("If there are {0} or more {1}ish wires, cut the wire after the first {2}ish wire.", parameters[0], Arrays.GroupedColors[parameters[1]], Arrays.GroupedColors[parameters[2]])
        };

        string method = parameters[2] < 5 ? "firstInstanceOfBlue" : "firstInstanceOfPurple";

        if (Algorithms.GetColors(grouped: true, wires: wires)[parameters[1] / 5] >= parameters[0])
        {
            condition.Wire = Algorithms.Find(method: method, key: ref parameters[0], wires: wires) + 1;

            if (condition.Wire > wires.Length)
                condition.Wire = null;
        }

        return condition;
    }

    public static Condition J(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(0, Arrays.Colors.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If there is only 1 {0} wire, cut that wire.", Arrays.Colors[parameter])
        };

        if (Algorithms.GetColors(grouped: false, wires: wires)[parameter] == 1)
            condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref parameter, wires: wires);

        return condition;
    }

    public static Condition K(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(0, Arrays.Colors.Length);
        bool ascending = rnd.NextDouble() > 0.5, first = rnd.NextDouble() > 0.5, odd = rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("If all values are in {0} order, cut the wire with the {1} {2} value.", ascending ? "ascending" : "decending", first ? "first" : "last", odd ? "odd" : "even")
        };

        int[] revertedWires = Algorithms.RevertLookup(wires, ref lookup);

        for (int i = 1; i < revertedWires.Length; i++)
            if ((ascending && revertedWires[i - 1] > revertedWires[i]) || (!ascending && revertedWires[i - 1] < revertedWires[i]))
                return condition;

        string method = first ? "first" : "last";
        method += odd ? "Odd" : "Even";

        condition.Wire = Algorithms.Find(method: method, key: ref parameter, wires: revertedWires);

        return condition;
    }

    public static Condition L(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: Arrays.Colors.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If a {0}ish wire neighbours 2 {1}ish wires, cut that middle wire.", Arrays.GroupedColors[parameters[0]], Arrays.GroupedColors[parameters[1]])
        };

        for (int i = 1; i < wires.Length - 1; i++)
            if (wires[i - 1] / 5 == parameters[1] / 5 && wires[i] / 5 == parameters[0] / 5 && wires[i + 1] / 5 == parameters[1] / 5)
            {
                condition.Wire = ++i;
                break;
            }

        return condition;
    }

    public static Condition M(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(0, wires.Length);
        bool highestIf = rnd.NextDouble() > 0.5, highestThen = rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("If the {0} wire has the {1} value, cut the first {2}-valued wire.", Arrays.Ordinals[parameter], highestIf ? "highest" : "lowest", highestThen ? "highest" : "lowest")
        };

        int[] revertedWires = Algorithms.RevertLookup(wires, ref lookup);

        if ((highestIf && revertedWires.Max() == revertedWires[parameter]) || 
           (!highestIf && revertedWires.Min() == revertedWires[parameter]))
            condition.Wire = highestThen ? revertedWires.ToList().IndexOf(revertedWires.Max()) + 1
                                         : revertedWires.ToList().IndexOf(revertedWires.Min()) + 1;

        return condition;
    }

    public static Condition N(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(0, wires.Length);
        bool first = rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("If there's a wire with a value difference of 5 from the {0} wire, cut the {1} wire matching that description.", Arrays.Ordinals[parameter], first ? "first" : "last")
        };

        string method = first ? "firstInstanceOfOppositeKey" : "lastInstanceOfOppositeKey";

        condition.Wire = Algorithms.Find(method: method, key: ref wires[parameter], wires: wires);

        return condition;
    }

    public static Condition O(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: Arrays.Colors.Length);
        parameters[0] = rnd.Next(0, wires.Length);
        bool first = rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("If the {0} wire is {1}, cut the {2} {1} wire.", Arrays.Ordinals[parameters[0]], Arrays.Colors[parameters[1]], first ? "first" : "last")
        };

        string method = first ? "firstInstanceOfKey" : "lastInstanceOfKey";

        if (wires[parameters[0]] == parameters[1])
            condition.Wire = Algorithms.Find(method: method, key: ref parameters[1], wires: wires);

        return condition;
    }

    public static Condition P(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(0, Arrays.Colors.Length);
        int divisible = rnd.Next(2, 7);

        Condition condition = new Condition
        {
            Text = string.Format("If the sum of all wire's values are divisible by {0}, cut the last {1}ish wire.", divisible, Arrays.GroupedColors[parameter])
        };

        int[] revertedWires = Algorithms.RevertLookup(wires, ref lookup);

        string method = parameter < 5 ? "lastInstanceOfPurple" : "lastInstanceOfBlue";

        if (revertedWires.Sum() % divisible == 0)
            condition.Wire = Algorithms.Find(method: method, key: ref parameter, wires: revertedWires);

        return condition;
    }

    public static Condition Q(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: wires.Length);
        bool higher = rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("If the {0} wire has a {1} value than the {2} wire, cut the first wire that has a value difference of 5 from any of the other wires.", Arrays.Ordinals[parameters[0]], higher ? "higher" : "lower", Arrays.Ordinals[parameters[1]])
        };

        int[] revertedWires = Algorithms.RevertLookup(wires, ref lookup);

        if ((higher && revertedWires[parameters[0]] > revertedWires[parameters[1]]) ||
           (!higher && revertedWires[parameters[0]] < revertedWires[parameters[1]]))
        {
            int?[] opposites = new int?[revertedWires.Length];

            for (int i = 0; i < revertedWires.Length; i++)
                opposites[i] = Algorithms.Find(method: "firstInstanceOfOppositeKey", key: ref revertedWires[i], wires: revertedWires);

            condition.Wire = Algorithms.First(opposites);
        }
        
        return condition;
    }

    public static Condition R(int[] wires, int lookup, KMBombInfo Info)
    {
        bool more = rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("If there are {0} blueish wires than purpleish wires, cut the last wire that has a value difference of 5 from any of the other wires.", more ? "more" : "less")
        };

        int[] colors = Algorithms.GetColors(grouped: true, wires: wires);

        if ((more && colors[0] > colors[1]) || (!more && colors[0] < colors[1]))
        {
            int?[] results = new int?[wires.Length];

            for (int i = 0; i < wires.Length; i++)
                results[i] = Algorithms.Find(method: "lastInstanceOfOppositeKey", key: ref wires[i], wires: wires);

            condition.Wire = results.Max();

            if (condition.Wire == 0)
                condition.Wire = null;
        }

        return condition;
    }

    public static Condition S(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(0, wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If an indicator exists containing one of the letters in \"Role\", cut the {0} wire.", Arrays.Ordinals[parameter])
        };

        Indicator[] indicators = new Indicator[] { Indicator.BOB, Indicator.CAR, Indicator.CLR, Indicator.FRK, Indicator.FRQ, Indicator.NLL, Indicator.TRN };

        foreach (Indicator indicator in indicators)
            if (Info.IsIndicatorPresent(indicator))
            {
                condition.Wire = ++parameter;
                break;
            }

        return condition;
    }

    public static Condition T(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(0, wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If there isn't only 1 module with their name containing \"Role Reversal\", cut the {0} wire, also thanks!", Arrays.Ordinals[parameter])
        };

        if (new Arrays(Info).GetNumbers(16) > 1)
            condition.Wire = ++parameter;

        return condition;
    }

    public static Condition U(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(0, wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If the serial number contains a letter in \"Reformed Role Reversal\", cut the {0} wire.", Arrays.Ordinals[parameter])
        };

        const string name = "ReformedRoleReversal";
        string serial = Info.GetSerialNumber();
        
        foreach (char n in name)
            if (serial.Contains(n))
            {
                condition.Wire = ++parameter;
                break;
            }

        return condition;
    }

    public static Condition V(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: Arrays.IndicatorNames.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If {0} indicator or {1} indicator exist, cut the wire matching the amount of indicators present.", Arrays.IndicatorNames[parameters[0]], Arrays.IndicatorNames[parameters[1]])
        };

        if (Info.IsIndicatorPresent(Arrays.Indicators[parameters[0]]) || Info.IsIndicatorPresent(Arrays.Indicators[parameters[1]]))
            condition.Wire = Info.GetIndicators().Count() > wires.Length ? null : (int?)Info.GetIndicators().Count();

        return condition;
    }

    public static Condition W(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 3, min: 0, max: Arrays.Colors.Length);
        parameters[0] = rnd.Next(0, Arrays.Edgework.Length);

        Condition condition = new Condition
        {
            Text = string.Format("If there are less {0} than {1} wires, cut the last non-{2}ish wire.", Arrays.Edgework[parameters[0]], Arrays.Colors[parameters[1]], Arrays.GroupedColors[parameters[1]])
        };

        string method = parameters[1] < 5 ? "lastInstanceOfPurple" : "lastInstanceOfBlue";

        if (new Arrays(Info).GetNumbers(parameters[0]) < wires.Where(x => x.Equals(parameters[0])).Count())
            condition.Wire = Algorithms.Find(method: method, key: ref parameters[1], wires: wires);

        return condition;
    }

    public static Condition X(int[] wires, int lookup, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: Arrays.Colors.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If there are less {0} than {1} wires, cut the first non-{1} wire.", Arrays.Edgework[parameters[0]], Arrays.Colors[parameters[1]])
        };

        if (new Arrays(Info).GetNumbers(parameters[0]) < wires.Where(x => x.Equals(parameters[1])).Count())
            condition.Wire = Algorithms.Find(method: "firstInstanceOfNotKey", key: ref parameters[1], wires: wires);

        return condition;
    }

    public static Condition Y(int[] wires, int lookup, KMBombInfo Info)
    {
        Condition condition = new Condition
        {
            Text = string.Format("If the serial number has a digit matching any amount of wires present excluding 0, cut the last wire that is most common.")
        };

        int[] colors = Algorithms.GetColors(grouped: false, wires: wires);
        IEnumerable<int> serial = Info.GetSerialNumberNumbers();

        for (int i = 0; i < colors.Length; i++)
            if (colors[i] != 0 && serial.Contains(colors[i]))
            {
                int[] maxWires = wires.ToLookup(n => n).ToLookup(l => l.Count(), l => l.Key).OrderBy(l => l.Key).Last().ToArray();
                int?[] results = new int?[maxWires.Length];

                for (int j = 0; j < results.Length; j++)
                    results[j] = (int)Algorithms.Find(method: "lastInstanceOfKey", key: ref maxWires[j], wires: wires);

                condition.Wire = results.Max() == 0 ? null : results.Max();
                break;
            }

        return condition;
    }

    public static Condition Z(int[] wires, int lookup, KMBombInfo Info)
    {
        Condition condition = new Condition
        {
            Text = string.Format("If the serial number has a digit matching the amount of the most common wires or total wires, cut the first wire that is least common.")
        };

        int[] colors = Algorithms.GetColors(grouped: false, wires: wires);
        IEnumerable<int> serial = Info.GetSerialNumberNumbers();

        if (serial.Contains(colors.Max()) || serial.Contains(wires.Length))
        {
            int[] maxWires = wires.ToLookup(n => n).ToLookup(l => l.Count(), l => l.Key).OrderBy(l => l.Key).First().ToArray();
            int?[] results = new int?[maxWires.Length];

            for (int j = 0; j < results.Length; j++)
                results[j] = (int)Algorithms.Find(method: "firstInstanceOfKey", key: ref maxWires[j], wires: wires);

            condition.Wire = Algorithms.First(results);
        }

        return condition;
    }
    #endregion

    #region Last Conditions
    public static Condition LastA(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = rnd.Next(0, wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("Cut the {0} wire.", Arrays.Ordinals[parameter])
        };

        condition.Wire = ++parameter;

        return condition;
    }

    public static Condition LastB(int[] wires, int lookup, KMBombInfo Info)
    {
        int parameter = wires[rnd.Next(0, wires.Length)];
        bool first = rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("Cut the {0} {1} wire.", first ? "first" : "last", Arrays.Colors[parameter])
        };

        condition.Wire = first 
                       ? Algorithms.Find(method: "firstInstanceOfKey", key: ref parameter, wires: wires)
                       : Algorithms.Find(method: "lastInstanceOfKey", key: ref parameter, wires: wires);

        return condition;
    }

    public static Condition LastC(int[] wires, int lookup, KMBombInfo Info)
    {
        bool highest = rnd.NextDouble() > 0.5, firstLast = rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("Cut the {0} {1}-valued wire.", highest ? "highest" : "lowest", firstLast ? "first" : "last")
        };

        int[] revertedWires = Algorithms.RevertLookup(wires, ref lookup);
        int key = highest ? revertedWires.Max() : revertedWires.Min();

        condition.Wire = firstLast 
                       ? Algorithms.Find(method: "firstInstanceOfKey", key: ref key, wires: revertedWires) 
                       : Algorithms.Find(method: "lastInstanceOfKey", key: ref key, wires: revertedWires);

        return condition;
    }
    #endregion
}
