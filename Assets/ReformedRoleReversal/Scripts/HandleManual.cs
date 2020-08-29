using System;
using System.Reflection;
using UnityEngine;
using System.Collections;
using Rnd = System.Random;
using System.Linq;

/// <summary>
/// Generates the module and caches the answer.
/// </summary>
internal class HandleManual
{
    internal HandleManual(HandleCoroutines coroutines, Init init)
    {
        this.coroutines = coroutines;
        this.init = init;
        reversal = init.Reversal;
        interact = init.Interact;
    }

    protected internal readonly int Seed = rnd.Next(0, 1000000000);

    private readonly HandleCoroutines coroutines;
    private readonly Init init;
    private readonly Interact interact;
    private readonly ReformedRoleReversal reversal;

    private Condition[] tutorial;
    private static readonly Rnd rnd = new Rnd();
    private int generated = 0;

    /// <summary>
    /// Converts the random number generated into wires, and a seed for the module to display.
    /// </summary>
    protected internal void Generate()
    {
        // The amount of wires is calculated with mod 7, then add 3.
        int[] wires = new int[(Seed % 7) + 3];
        string strSeed = Seed.ToString();

        // Generate random parameters as rules.
        bool left = rnd.NextDouble() > 0.5, leftmost = rnd.NextDouble() > 0.5;

        // 10% of the time, the string is less than 9 characters long. Append accordingly.
        while (strSeed.Length < 9)
            strSeed = left ? '0' + strSeed : strSeed + '0';

        // Random lookup table, which is simply the default with all values added to this variable.
        int lookup = rnd.Next(0, 10);

        for (int i = 0; i < wires.Length; i++)
            wires[i] = (int)(char.GetNumericValue(strSeed[leftmost ? i : i + (9 - wires.Length)]) + lookup) % 10;

        // Get random base. Base 20 is minimum because it never displays more than 7 characters.
        char[] baseN = Algorithms.SubArray(Arrays.Base62, rnd.Next(20, 63));

        // Converts the seed from base 10 to the random base chosen.
        reversal.SeedText.text = "Seed: " + Algorithms.ConvertFromBase10(value: int.Parse(strSeed), baseChars: baseN);
        
        Debug.LogFormat("[Reformed Role Reversal #{0}]: {1} -> Seed in Base {2}: {3}. Seed in Base 10: {4}. # of wires: {5}. Place {6} 0's. Take {7} wires. Lookup: #{8}.", init.ModuleId, Arrays.Version, baseN.Length, reversal.SeedText.text.Substring(6, reversal.SeedText.text.Length - 6), strSeed, wires.Length, left ? "left" : "right", leftmost ? "leftmost" : "rightmost", lookup);

        // Log the list of all wires, converting each index to the respective string.
        string[] log = new string[wires.Length];

        for (int i = 0; i < wires.Length; i++)
            log[i] += i == wires.Length - 1 ? "and " + Arrays.Colors[wires[i]] : Arrays.Colors[wires[i]];

        Debug.LogFormat("[Reformed Role Reversal #{0}]: The wires are {1}.", init.ModuleId, log.Join(", "));

        int i2 = init.Conditions.GetLength(0), j2 = init.Conditions.GetLength(1);

        // Formats the tutorial, this needs to run before the conditions are generated because it assigns the first set using this variable.
        tutorial = new Arrays(reversal.Info).GetTutorial(interact.ButtonOrder, baseN.Length, ref left, ref leftmost, ref lookup);

        // Runs through the entire 2-dimensional array and assign a condition to each and every single one.
        for (int i = 0; i < i2; i++)
            for (int j = 0; j < j2; j++)
                coroutines.GenerateCondition(i, j, wires, ref strSeed, ref lookup);
    }

    /// <summary>
    /// Generates a condition and sets the currently assigned variable to it.
    /// </summary>
    /// <param name="i">The index of the first dimension.</param>
    /// <param name="j">The index of the second dimension.</param>
    /// <param name="wires">The list of wires.</param>
    /// <param name="strSeed">The seed converted to a string, very similar to wires.</param>
    /// <param name="lookup">This variable is needed in case if the lookup offset needs to be reverted.</param>
    /// <param name="isCorrectIndex">To prevent having the user find out the amount of wires by carefully reading the conditions, the wires specified are adjusted per section.</param>
    /// <returns>This is meant for multithreading, and only returns null.</returns>
    protected internal IEnumerator GenerateCondition(int i, int j, int[] wires, string strSeed, int lookup, bool isCorrectIndex)
    {
        yield return null;

        // If the current condition is in the tutorial section, assign it to the tutorial already generated before.
        if (i == 0)
        {
            init.Conditions[i, j] = tutorial[j];

            // Theoretically generateCondition++ could run before the previous instruction has finished running.
            yield return new WaitWhile(() => init.Conditions[i, j] == null);
            generated++;

            yield break;
        }

        // Generates fake wires for sections with incorrect amount of wires to obfuscate real ones based on the conditions recieved.
        if (!isCorrectIndex)
            wires = Enumerable.Repeat(0, i + 2).Select(k => rnd.Next(0, 10)).ToArray();

        // Contains all methods in the Manual class.
        const string randomMethods = "ABCDEFGHIJKLMNOPQRSTUVWXYZ", randomFirstLastMethods = "ABC";
        Type classType = typeof(Manual);
        MethodInfo methodInfo;

        switch (j)
        {
            // First case. (Guaranteed edgework)
            case 0: methodInfo = classType.GetMethod("First" + randomFirstLastMethods[rnd.Next(0, randomFirstLastMethods.Length)].ToString()); break;

            // Last case. (Guaranteed no edgework)
            case 7: methodInfo = classType.GetMethod("Last" + randomFirstLastMethods[rnd.Next(0, randomFirstLastMethods.Length)].ToString()); break;

            // Every other case. (Mixed)
            default: methodInfo = classType.GetMethod(randomMethods[rnd.Next(0, randomMethods.Length)].ToString()); break;
        }
        
        // Invoke the random method obtained and assign it into the current variable.
        init.Conditions[i, j] = (Condition)methodInfo.Invoke(this, new object[] { wires, lookup, reversal.Info });

        // Wait until the method has finished running.
        yield return new WaitWhile(() => init.Conditions[i, j] == null);
        generated++;

        // If this is the last time the coroutine is running, get the answer, and consider the module ready.
        if (generated == init.Conditions.GetLength(0) * init.Conditions.GetLength(1))
        {
            interact.CorrectAnswer = GetAnswer(ref strSeed);
            Init.LightsOn = true;
        }
    }

    /// <summary>
    /// Scans through the condition's Wire and SkipTo properties to determine the answer of the module.
    /// </summary>
    /// <param name="strSeed">The seed in base 10.</param>
    /// <returns>Returns the answer, if the answer is null then any wire can be cut.</returns>
    private int? GetAnswer(ref string strSeed)
    {
        int wireSelected = 1, wireCount = (int.Parse(strSeed) % 7) + 1, iMax = init.Conditions.GetLength(1);
        bool isSelectingWire = false;

        coroutines.UpdateScreen(instructionX: 0, instructionY: 0, wireSelected: ref wireSelected, isSelectingWire: ref isSelectingWire);

        for (int i = 0; i < iMax; i++)
        {
            // If true, set the current index to the SkipTo property.
            if (init.Conditions[wireCount, i].SkipTo != null)
            {
                if (init.Conditions[wireCount, i].SkipTo < 1 || init.Conditions[wireCount, i].SkipTo > iMax)
                    throw new IndexOutOfRangeException("[Reformed Role Reversal #" + init.ModuleId + "]: Condition [" + wireCount + ", " + i + "] returned " + init.Conditions[wireCount, i].SkipTo + " for parameter \"SkipTo\"! This should not happen under normal circumstances, as the specified condition doesn't exist.");

                Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is true, skip to section {4}.", init.ModuleId, wireCount + 2, i + 1, init.Conditions[wireCount, i].Text, init.Conditions[wireCount, i].SkipTo);
                i = (int)init.Conditions[wireCount, i].SkipTo - 1;
            }

            // If true, the answer has been reached, and the wire to cut is in the Wire property.
            if (init.Conditions[wireCount, i].Wire != null)
            {
                if (init.Conditions[wireCount, i].Wire < 1 || init.Conditions[wireCount, i].Wire > 9)
                    throw new IndexOutOfRangeException("[Reformed Role Reversal #" + init.ModuleId + "]: Condition [" + (wireCount + 2) + ", " + (i + 1) + "] returned " + init.Conditions[wireCount, i].Wire + " for parameter \"Wire\"! This should not happen under normal circumstances, as the wire specified to cut doesn't exist.");

                Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is true, cut the {4} wire.", init.ModuleId, wireCount + 2, i + 1, init.Conditions[wireCount, i].Text, Arrays.Ordinals[(int)init.Conditions[wireCount, i].Wire - 1]);
                return (int)init.Conditions[wireCount, i].Wire;
            }

            Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is false.", init.ModuleId, wireCount + 2, i + 1, init.Conditions[wireCount, i].Text);
        }
        
        // Failsafe: If the answer isn't found, any wire can be cut.
        Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> Unreachable code detected, please cut any wire to solve the module.", init.ModuleId);
        return null;
    }
}
