using KModkit;
using System;
using System.Collections.Generic;
using System.Linq;

static class Manual
{
    private static readonly Random Rnd = new Random();

    #region First Conditions
    public static Condition FirstA(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 3, min: 0, max: StaticArrays.Edgework.Length);
        int edgework = new StaticArrays(Info).GetNumbers(parameters[1]);
        bool inversion = Rnd.NextDouble() > 0.5;

        parameters[0] = (parameters[0] / 5) + 2;
        parameters[2] = (parameters[2] % 2) + 3;

        Condition condition = new Condition
        {
            Text = string.Format("If there's at {0} {1} {2}, skip to condition {3}.", inversion ? "more" : "least", parameters[0], StaticArrays.Edgework[parameters[1]], parameters[2])
        };

        if ((!inversion && parameters[0] <= edgework) || (inversion && parameters[0] >= edgework))
            condition.SkipTo = parameters[2];

        return condition;
    }

    public static Condition FirstB(int[] wires, KMBombInfo Info)
    {
        StaticArrays staticArrays = new StaticArrays(Info);

        int[] parameters = Algorithms.Random(length: 3, min: 0, max: StaticArrays.Edgework.Length);
        int edgework1 = staticArrays.GetNumbers(parameters[0]), edgework2 = staticArrays.GetNumbers(parameters[1]);
        bool more = Rnd.NextDouble() > 0.5;

        parameters[2] = (parameters[2] % 2) + 3;

        Condition condition = new Condition
        {
            Text = string.Format("If there's {0} {1} than {2}, skip to condition {3}.", more ? "more" : "less", StaticArrays.Edgework[parameters[0]], StaticArrays.Edgework[parameters[1]], parameters[2])
        };

        if ((!more && edgework1 < edgework2) || (more && edgework1 > edgework2))
            condition.SkipTo = parameters[2];

        return condition;
    }

    public static Condition FirstC(int[] wires, KMBombInfo Info)
    {
        StaticArrays staticArrays = new StaticArrays(Info);

        int[] parameters = Algorithms.Random(length: 3, min: 0, max: StaticArrays.Edgework.Length);
        int edgework1 = staticArrays.GetNumbers(parameters[0]), edgework2 = staticArrays.GetNumbers(parameters[1]);
        bool orAnd = Rnd.NextDouble() > 0.5, inversion = Rnd.NextDouble() > 0.5;

        parameters[2] = (parameters[2] % 2) + 3;

        Condition condition = new Condition
        {
            Text = string.Format("If {0} {1} {2} {3} exist, skip to condition {4}.", StaticArrays.Edgework[parameters[0]], orAnd ? "or" : "and", StaticArrays.Edgework[parameters[1]], inversion ? "don't" : "do", parameters[2])
        };

        if (inversion && ((orAnd && (edgework1 == 0 || edgework2 == 0)) || (!orAnd && edgework1 == 0 && edgework2 == 0))
        || !inversion && ((orAnd && (edgework1 != 0 || edgework2 != 0)) || (!orAnd && edgework1 != 0 && edgework2 != 0)))
            condition.SkipTo = parameters[2];

        return condition;
    }
    #endregion

    #region Noninitial Conditions
    public static Condition A(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: StaticArrays.Colors.Length);
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
        int[] parameters = Algorithms.Random(length: 4, min: 0, max: StaticArrays.Colors.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If a {0} wire is left of a {1}, {2}, or {3} wire, cut the first {1}, {2}, or {3} wire.", StaticArrays.Colors[parameters[0]], StaticArrays.Colors[parameters[1]], StaticArrays.Colors[parameters[2]], StaticArrays.Colors[parameters[3]])
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

    public static Condition C(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 3, min: 0, max: wires.Length + 1);

        Condition condition = new Condition
        {
            Text = string.Format("If the {0}, {1}, or {2} wire share any color, cut the first wire that isn't the shared color.", StaticArrays.Ordinals[parameters[0]], StaticArrays.Ordinals[parameters[1]], StaticArrays.Ordinals[parameters[2]])
        };
        
        if (wires[parameters[0]] == wires[parameters[1]] || wires[parameters[1]] == wires[parameters[2]] || wires[parameters[2]] == wires[parameters[0]])
        {
            int matchingWire = wires[parameters[0]] == wires[parameters[1]] ? wires[parameters[0]] : wires[parameters[2]];
            condition.Wire = Algorithms.Find(method: "firstInstanceOfNotKey", key: ref matchingWire, wires: wires);
        }

        return condition;
    }

    public static Condition D(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next((int)Math.Ceiling((float)wires.Length / 2), wires.Length);
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

    public static Condition E(int[] wires, KMBombInfo Info)
    {
        Condition condition = new Condition
        {
            Text = string.Format("If all wires are unique, cut the wire with the highest value.")
        };

        if (wires.Distinct().Count() == wires.Count())
        {
            int highestValue = wires.Max();
            condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref highestValue, wires: wires);
        }

        return condition;
    }

    public static Condition F(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(1, wires.Length - 1);
        bool seenNotUnique = false;

        Condition condition = new Condition
        {
            Text = string.Format("If all wires are unique except for 1 {0} with matching colors, cut the first wire with the lowest value.", StaticArrays.Tuplets[parameter])
        };

        foreach (IGrouping<int, int> number in wires.GroupBy(x => x))
        {
            if (number.Count() == 1)
                continue;
            
            if (number.Count() == parameter + 1 && !seenNotUnique)
            {
                seenNotUnique = true;
                continue;
            }

            return condition;
        }

        if (seenNotUnique)
        {
            int lowestWire = wires.Min();
            condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref lowestWire, wires: wires);
        }

        return condition;
    }

    public static Condition G(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(1, (wires.Length / 2) + 1);
        Array.Sort(wires);

        int exceptions = 0,
            middleWire1 = wires[wires.Length / 2],
            middleWire2 = wires.Length % 2 == 0
                        ? wires[(int)(((float)wires.Length / 2) + 0.6f)]
                        : middleWire1;

        Condition condition = new Condition
        {
            Text = string.Format("If all wires aren't unique excluding {0}, cut the last wire with the median(s).", parameter)
        };

        foreach (IGrouping<int, int> number in wires.GroupBy(x => x))
        {
            if (number.Count() != 1)
                continue;

            exceptions++;

            if (exceptions > parameter)
                return condition;
        }

        condition.Wire = Math.Max((int)Algorithms.Find(method: "lastInstanceOfKey", key: ref middleWire1, wires: wires),
                                  (int)Algorithms.Find(method: "lastInstanceOfKey", key: ref middleWire2, wires: wires));

        return new Condition { Wire = condition.Wire == 0 ? null : condition.Wire, SkipTo = condition.SkipTo, Text = condition.Text };
    }

    public static Condition H(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: StaticArrays.Colors.Length);
        bool inversion = Rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("If there {0} any {1} wires, cut the first {2}ish wire.", inversion ? "aren't" : "are", StaticArrays.Colors[parameters[0]], StaticArrays.GroupedColors[parameters[1]])
        };

        string method = parameters[1] < 5 ? "firstInstanceOfPurple" : "firstInstanceOfBlue";

        if (inversion ^ wires.Contains(parameters[0]))
            condition.Wire = Algorithms.Find(method: method, key: ref parameters[0], wires: wires);

        return condition;
    }

    public static Condition I(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 3, min: 0, max: StaticArrays.GroupedColors.Length);
        parameters[0] = Rnd.Next((int)Math.Ceiling((float)wires.Length / 2), wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If there are {0} or more {1}ish wires, cut the wire after the first {2}ish wire.", parameters[0], StaticArrays.GroupedColors[parameters[1]], StaticArrays.GroupedColors[parameters[2]])
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

    public static Condition J(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(0, StaticArrays.Colors.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If there is only 1 {0} wire, cut that wire.", StaticArrays.GroupedColors[parameter])
        };

        if (Algorithms.GetColors(grouped: false, wires: wires)[parameter] == 1)
            condition.Wire = Algorithms.Find(method: "firstInstanceOfKey", key: ref parameter, wires: wires);

        return condition;
    }

    public static Condition K(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(0, StaticArrays.Colors.Length);
        bool ascending = Rnd.NextDouble() > 0.5, highest = Rnd.NextDouble() > 0.5, odd = Rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("If all values are in {0} order, cut the wire with the {1} {2} value.", ascending ? "ascending" : "decending", highest ? "highest" : "lowest", odd ? "odd" : "even")
        };

        for (int i = 1; i < wires.Length; i++)
        {
            if ((ascending && wires[i - 1] > wires[i]) || (!ascending && wires[i - 1] < wires[i]))
                break;

            if (i == wires.Length - 1)
            {
                string method = highest ? "highest" : "lowest";
                method += odd ? "Odd" : "Even";
                condition.Wire = Algorithms.Find(method: method, key: ref parameter, wires: wires);
            }
        }

        return condition;
    }

    public static Condition L(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: StaticArrays.Colors.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If a {0}ish wire neighbours 2 {1}ish wires, cut that middle wire.", StaticArrays.GroupedColors[parameters[0]], StaticArrays.GroupedColors[parameters[1]])
        };

        for (int i = 1; i < wires.Length - 1; i++)
            if (wires[i - 1] / 5 == parameters[1] / 5 && wires[i] / 5 == parameters[0] / 5 && wires[i + 1] / 5 == parameters[1] / 5)
            {
                condition.Wire = ++i;
                break;
            }

        return condition;
    }

    public static Condition M(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(0, wires.Length);
        bool highestIf = Rnd.NextDouble() > 0.5, highestThen = Rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("If the {0} wire has the {1} value, cut the first {2}-valued wire.", StaticArrays.Ordinals[parameter], highestIf ? "highest" : "lowest", highestThen ? "highest" : "lowest")
        };

        if ((highestIf && wires.Max() == wires[parameter]) || (!highestIf && wires.Min() == wires[parameter]))
            condition.Wire = highestThen ? wires.ToList().IndexOf(wires.Max()) : wires.ToList().IndexOf(wires.Min());

        return condition;
    }

    public static Condition N(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(0, wires.Length);
        bool first = Rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("If there's a wire with a value difference of 5 from the {0} wire, cut the {1} wire matching that description.", StaticArrays.Ordinals[parameter], first ? "first" : "last")
        };

        string method = first ? "firstInstanceOfOppositeKey" : "lastInstanceOfOppositeKey";

        condition.Wire = Algorithms.Find(method: method, key: ref wires[parameter], wires: wires);

        return condition;
    }

    public static Condition O(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: StaticArrays.Colors.Length);
        parameters[0] = Rnd.Next(0, wires.Length);
        bool first = Rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("If the {0} wire is {1}, cut the {2} {1} wire.", StaticArrays.Ordinals[parameters[0]], StaticArrays.Colors[parameters[1]], first ? "first" : "last")
        };

        string method = first ? "firstInstanceOfKey" : "lastInstanceOfKey";

        if (wires[parameters[0]] == parameters[1])
            condition.Wire = Algorithms.Find(method: method, key: ref parameters[1], wires: wires);

        return condition;
    }

    public static Condition P(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(0, StaticArrays.Colors.Length);
        int divisible = Rnd.Next(2, 7);
        Condition condition = new Condition
        {
            Text = string.Format("If the sum of all wire's values are divisible by {0}, cut the last {1}ish wire.", divisible, StaticArrays.GroupedColors[parameter])
        };

        string method = parameter < 5 ? "lastInstanceOfPurple" : "lastInstanceOfBlue";

        if (wires.Sum() % divisible == 0)
            condition.Wire = Algorithms.Find(method: method, key: ref parameter, wires: wires);

        return condition;
    }

    public static Condition Q(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: StaticArrays.Colors.Length);
        bool higher = Rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("If the {0} wire has a {1} value than the {2} wire, cut the first wire that has a value difference of 5 from any of the other wires.", StaticArrays.Ordinals[parameters[0]], higher ? "higher" : "lower", StaticArrays.Ordinals[parameters[1]])
        };

        if ((higher && wires[parameters[0]] > wires[parameters[1]]) || (!higher && wires[parameters[0]] < wires[parameters[1]]))
        {
            List<int> opposites = new List<int>(0);

            for (int i = 0; i < wires.Length; i++)
                opposites.Add((int)Algorithms.Find(method: "firstInstanceOfOppositeKey", key: ref wires[i], wires: wires));

            condition.Wire = Algorithms.First(opposites);

            if (condition.Wire == 0)
                condition.Wire = null;
        }
        
        return condition;
    }

    public static Condition R(int[] wires, KMBombInfo Info)
    {
        bool more = Rnd.NextDouble() > 0.5;
        Condition condition = new Condition
        {
            Text = string.Format("If there are {0} blueish wires than purpleish wires, cut the last wire that has a value difference of 5 from any of the other wires.", more ? "more" : "less")
        };

        int[] colors = Algorithms.GetColors(grouped: true, wires: wires);

        if ((more && colors[0] > colors[1]) || (!more && colors[0] < colors[1]))
        {
            List<int> opposites = new List<int>(0);

            for (int i = 0; i < wires.Length; i++)
                opposites.Add((int)Algorithms.Find(method: "lastInstanceOfOppositeKey", key: ref wires[i], wires: wires));

            condition.Wire = opposites.Max();

            if (condition.Wire == 0)
                condition.Wire = null;
        }

        return condition;
    }

    public static Condition S(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(0, wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If an indicator exists containing one of the letters in \"Role\", cut the {0} wire.", StaticArrays.Ordinals[parameter])
        };

        foreach (Indicator indicator in new Indicator[] { Indicator.BOB, Indicator.CAR, Indicator.CLR, Indicator.FRK, Indicator.FRQ, Indicator.NLL, Indicator.TRN})
            if (Info.IsIndicatorPresent(indicator))
            {
                condition.Wire = ++parameter;
                break;
            }

        return condition;
    }

    public static Condition T(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(0, wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If there isn't only 1 module with their name containing \"Role Reversal\", cut the {0} wire, also thanks!", StaticArrays.Ordinals[parameter])
        };

        if (new StaticArrays(Info).GetNumbers(16) > 1)
            condition.Wire = ++parameter;

        return condition;
    }

    public static Condition U(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(0, wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If the serial number contains a letter in \"Reformed Role Reversal\", cut the {0} wire.", StaticArrays.Ordinals[parameter])
        };

        const string name = "ReformedRoleReversal";

        for (int i = 0; i < name.Length; i++)
            if (Info.GetSerialNumber().Contains(name[i]))
            {
                condition.Wire = ++parameter;
                break;
            }

        return condition;
    }

    public static Condition V(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: StaticArrays.IndicatorNames.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If {0} indicator or {1} indicator exist, cut the wire matching the amount of indicators present.", StaticArrays.IndicatorNames[parameters[0]], StaticArrays.IndicatorNames[parameters[1]])
        };

        if (Info.IsIndicatorPresent(StaticArrays.Indicators[parameters[0]]) || Info.IsIndicatorPresent(StaticArrays.Indicators[parameters[1]]))
            condition.Wire = Info.GetIndicators().Count() > wires.Length ? null : (int?)Info.GetIndicators().Count();

        return condition;
    }

    public static Condition W(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 3, min: 0, max: StaticArrays.Colors.Length);
        parameters[0] = Rnd.Next(0, StaticArrays.Edgework.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If there are less {0} than {1} wires, cut the last non-{2}ish wire.", StaticArrays.Edgework[parameters[0]], StaticArrays.Colors[parameters[1]], StaticArrays.GroupedColors[parameters[1]])
        };

        int matches = 0;

        for (int i = 0; i < wires.Length; i++)
            if (wires[i] == parameters[0])
                matches++;

        string method = parameters[1] < 5 ? "lastInstanceOfPurple" : "lastInstanceOfBlue";

        if (new StaticArrays(Info).GetNumbers(parameters[0]) < matches)
            condition.Wire = Algorithms.Find(method: method, key: ref parameters[1], wires: wires);

        return condition;
    }

    public static Condition X(int[] wires, KMBombInfo Info)
    {
        int[] parameters = Algorithms.Random(length: 2, min: 0, max: StaticArrays.Colors.Length);
        Condition condition = new Condition
        {
            Text = string.Format("If there are less {0} than {1} wires, cut the first non-{1} wire.", StaticArrays.Edgework[parameters[0]], StaticArrays.Colors[parameters[1]])
        };

        int matches = 0;

        for (int i = 0; i < wires.Length; i++)
            if (wires[i] == parameters[0])
                matches++;

        if (new StaticArrays(Info).GetNumbers(parameters[0]) < matches)
            condition.Wire = Algorithms.Find(method: "firstInstanceOfNotKey", key: ref parameters[1], wires: wires);

        return condition;
    }

    public static Condition Y(int[] wires, KMBombInfo Info)
    {
        Condition condition = new Condition
        {
            Text = string.Format("If the serial number has a digit matching any amount of wires present excluding 0, cut the last wire that is most common.")
        };

        int[] colors = Algorithms.GetColors(grouped: false, wires: wires);

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
                        potentialWires[k] = (int)Algorithms.Find(method: "lastInstanceOfKey", key: ref check, wires: wires);
                        condition.Wire = Algorithms.First(potentialWires);
                    }
                }

        return condition;
    }

    public static Condition Z(int[] wires, KMBombInfo Info)
    {
        Condition condition = new Condition
        {
            Text = string.Format("If the serial number has a digit matching the amount of the most common wires or total wires, cut the first wire that is least common.")
        };

        int[] colors = Algorithms.GetColors(grouped: false, wires: wires);

        if (Info.GetSerialNumberNumbers().Contains(colors.Max()) || Info.GetSerialNumberNumbers().Contains(wires.Length))
            for (int i = 1; i <= 9; i++)
            {
                List<int> potentialWires = new List<int>(0);

                for (int j = 0; j < colors.Length; j++)
                    if (colors[j] == i)
                        potentialWires.Add(j);

                for (int j = 0; j < potentialWires.Count; j++)
                {
                    int check = potentialWires[j];
                    potentialWires[j] = (int)Algorithms.Find(method: "firstInstanceOfKey", key: ref check, wires: wires);
                    condition.Wire = Algorithms.First(potentialWires);
                }
            }

        return condition;
    }
    #endregion

    #region Last Conditions
    public static Condition LastA(int[] wires, KMBombInfo Info)
    {
        int parameter = Rnd.Next(0, wires.Length);
        Condition condition = new Condition
        {
            Text = string.Format("Cut the {0} wire.", StaticArrays.Ordinals[parameter])
        };

        condition.Wire = ++parameter;

        return condition;
    }

    public static Condition LastB(int[] wires, KMBombInfo Info)
    {
        int parameter = wires[Rnd.Next(0, wires.Length)];
        bool first = Rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("Cut the {0} {1} wire.", first ? "first" : "last", StaticArrays.Colors[parameter])
        };

        condition.Wire = first 
                       ? Algorithms.Find(method: "firstInstanceOfKey", key: ref parameter, wires: wires)
                       : Algorithms.Find(method: "lastInstanceOfKey", key: ref parameter, wires: wires);

        return condition;
    }

    public static Condition LastC(int[] wires, KMBombInfo Info)
    {
        bool highest = Rnd.NextDouble() > 0.5, firstLast = Rnd.NextDouble() > 0.5;

        Condition condition = new Condition
        {
            Text = string.Format("Cut the {0} {1}-valued wire.", highest ? "highest" : "lowest", firstLast ? "first" : "last")
        };

        int key = highest ? wires.Max() : wires.Min();

        condition.Wire = firstLast 
                       ? Algorithms.Find(method: "firstInstanceOfKey", key: ref key, wires: wires) 
                       : Algorithms.Find(method: "lastInstanceOfKey", key: ref key, wires: wires);

        return condition;
    }
    #endregion
}
