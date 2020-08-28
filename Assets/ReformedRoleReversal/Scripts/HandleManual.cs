using System;
using System.Reflection;
using UnityEngine;
using System.Collections;

/// <summary>
/// Generates the module and caches the answer.
/// </summary>
internal class HandleManual
{
    internal HandleManual(HandleCoroutines coroutines, Init init)
    {
        _coroutines = coroutines;
        _init = init;
        _reversal = _init.Reversal;
    }

    protected internal int? CorrectAnswer;
    protected internal readonly int Seed = Rnd.Next(0, 1000000000);

    private readonly HandleCoroutines _coroutines;
    private readonly Init _init;
    private readonly Interact _interact;
    private readonly ReformedRoleReversal _reversal;

    private Condition[] _tutorial;
    private static readonly System.Random Rnd = new System.Random();
    private int _generateCondition = 0;

    /// <summary>
    /// Converts the random number generated into wires, and a seed for the module to display.
    /// </summary>
    protected internal void Generate()
    {
        // The amount of wires is calculated with mod 7, then add 3.
        int[] wires = new int[(Seed % 7) + 3];
        string strSeed = Seed.ToString();

        // Generate random parameters as rules.
        bool left = Rnd.NextDouble() > 0.5, leftmost = Rnd.NextDouble() > 0.5;

        // 10% of the time, the string is less than 9 characters long. Append accordingly.
        while (strSeed.Length < 9)
            strSeed = left ? '0' + strSeed : strSeed + '0';

        // Random lookup table, which is simply the default with all values added to this variable.
        int lookup = Rnd.Next(0, 10);

        for (int i = 0; i < wires.Length; i++)
            wires[i] = (int)(char.GetNumericValue(strSeed[leftmost ? i : i + (9 - wires.Length)]) + lookup) % 10;

        // Get random base. Base 20 is minimum because it never displays more than 7 characters.
        char[] baseN = Algorithms.SubArray(Arrays.Base62, Rnd.Next(20, 63));

        // Converts the seed from base 10 to the random base chosen.
        _reversal.SeedText.text = "Seed: " + Algorithms.ConvertFromBase10(value: int.Parse(strSeed), baseChars: baseN);
    
        Debug.LogFormat("[Reformed Role Reversal #{0}]: Seed in Base {1}: {2} - Seed in Base 10: {3} - # of wires: {4}.", _init.ModuleId, baseN.Length, _reversal.SeedText.text.Substring(6, _reversal.SeedText.text.Length - 6), strSeed, wires.Length);
        Debug.LogFormat("[Reformed Role Reversal #{0}]: Append 0's on the left: {1}, grab leftmost wires: {2}, using lookup {3}.", _init.ModuleId, left, leftmost, lookup);

        // Log the list of all wires, converting each index to the respective string.
        string[] log = new string[wires.Length];

        for (int i = 0; i < wires.Length; i++)
            log[i] += i == wires.Length - 1 ? "and " + Arrays.Colors[wires[i]] : Arrays.Colors[wires[i]];

        Debug.LogFormat("[Reformed Role Reversal #{0}]: The wires are {1}.", _init.ModuleId, log.Join(", "));

        int i2 = _init.Conditions.GetLength(0), j2 = _init.Conditions.GetLength(1);

        // Formats the tutorial, this needs to run before the conditions are generated because it assigns the first set using this variable.
        _tutorial = new Arrays(_reversal.Info).GetTutorial(_init.Interact.ButtonOrder, baseN.Length, ref left, ref leftmost, ref lookup);

        // Runs through the entire 2-dimensional array and assign a condition to each and every single one.
        for (int i = 0; i < i2; i++)
            for (int j = 0; j < j2; j++)
                _coroutines.GenerateCondition(i, j, wires, ref strSeed, ref lookup);
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
            _init.Conditions[i, j] = _tutorial[j];

            // Theoretically _generateCondition++ could run before the previous instruction has finished running.
            yield return new WaitWhile(() => _init.Conditions[i, j] == null);
            _generateCondition++;

            yield break;
        }

        // Generates fake wires for sections with incorrect amount of wires to obfuscate real ones based on the conditions recieved.
        if (!isCorrectIndex)
        {
            while (i + 2 != wires.Length)
            {
                if (i + 2 > wires.Length)
                    wires = new int[wires.Length + 1];

                else
                    wires = new int[wires.Length - 1];
            }

            for (int k = 0; k < wires.Length; k++)
                wires[k] = Rnd.Next(0, 10);
        }

        // Contains all methods in the Manual class.
        const string randomMethods = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        Type classType = typeof(Manual);
        MethodInfo methodInfo;

        switch (j)
        {
            // First case. (Guaranteed edgework)
            case 0: methodInfo = classType.GetMethod("First" + randomMethods[Rnd.Next(0, 3)].ToString()); break;

            // Last case. (Guaranteed no edgework)
            case 7: methodInfo = classType.GetMethod("Last" + randomMethods[Rnd.Next(0, 3)].ToString()); break;

            // Every other case. (Mixed)
            default: methodInfo = classType.GetMethod(randomMethods[Rnd.Next(0, randomMethods.Length)].ToString()); break;
        }
        
        // Invoke the random method obtained and assign it into the current variable.
        _init.Conditions[i, j] = (Condition)methodInfo.Invoke(this, new object[] { wires, lookup, _reversal.Info });

        // Wait until the method has finished running.
        yield return new WaitWhile(() => _init.Conditions[i, j] == null);
        _generateCondition++;

        // If this is the last time the coroutine is running, get the answer, and consider the module ready.
        if (_generateCondition == _init.Conditions.GetLength(0) * _init.Conditions.GetLength(1))
        {
            CorrectAnswer = GetAnswer(ref strSeed);
            Init.LightsOn = true;
        }
    }

    private int? GetAnswer(ref string strSeed)
    {
        int wireSelected = 1;
        bool isSelectingWire = false;
        _coroutines.UpdateScreen(instructionX: 0, instructionY: 0, wireSelected: ref wireSelected, isSelectingWire: ref isSelectingWire);

        int wires = (int.Parse(strSeed) % 7) + 1, i2 = _init.Conditions.GetLength(1);

        for (int i = 0; i < i2; i++)
        {
            if (_init.Conditions[wires, i].SkipTo != null)
            {
                if (_init.Conditions[wires, i].SkipTo < 1 || _init.Conditions[wires, i].SkipTo > i2)
                    throw new IndexOutOfRangeException("[Reformed Role Reversal #" + _init.ModuleId + "]: Condition [" + wires + ", " + i + "] returned " + _init.Conditions[wires, i].SkipTo + " for parameter \"SkipTo\"! This should not happen under normal circumstances, as the specified condition doesn't exist.");

                Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is true, skip to section {4}.", _init.ModuleId, wires + 2, i + 1, _init.Conditions[wires, i].Text, _init.Conditions[wires, i].SkipTo);
                i = (int)_init.Conditions[wires, i].SkipTo - 1;
            }

            else if (_init.Conditions[wires, i].Wire != null)
            {
                if (_init.Conditions[wires, i].Wire < 1 || _init.Conditions[wires, i].Wire > 9)
                    throw new IndexOutOfRangeException("[Reformed Role Reversal #" + _init.ModuleId + "]: Condition [" + (wires + 2) + ", " + (i + 1) + "] returned " + _init.Conditions[wires, i].Wire + " for parameter \"Wire\"! This should not happen under normal circumstances, as the wire specified to cut doesn't exist.");

                Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is true, cut the {4} wire.", _init.ModuleId, wires + 2, i + 1, _init.Conditions[wires, i].Text, Arrays.Ordinals[(int)_init.Conditions[wires, i].Wire - 1]);
                return (int)_init.Conditions[wires, i].Wire;
            }

            Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is false.", _init.ModuleId, wires + 2, i + 1, _init.Conditions[wires, i].Text);
        }
        
        Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> Unreachable code detected, please cut any wire to solve the module.", _init.ModuleId);
        return null;
    }
}
