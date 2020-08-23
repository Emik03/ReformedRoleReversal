using System;
using System.Reflection;
using UnityEngine;
using System.Collections;

internal class HandleManual
{
    internal HandleManual(HandleCoroutines coroutines, Init init)
    {
        _coroutines = coroutines;
        _init = init;
        _roleReversal = init.RoleReversal;
    }

    protected internal readonly int Seed = Rnd.Next(0, 1000000000);

    private readonly HandleCoroutines _coroutines;
    private readonly Init _init;
    private readonly ReformedRoleReversal _roleReversal;

    private Condition[] _tutorial;
    private static readonly System.Random Rnd = new System.Random();
    private int _generateCondition = 0;

    /// <summary>
    /// Converts the random number generated into wires, and a seed for the module to display.
    /// </summary>
    protected internal void Generate()
    {
        int[] wires = new int[(Seed % 7) + 3];

        string strSeed = Seed.ToString();
        bool left = Rnd.NextDouble() > 0.5, leftmost = Rnd.NextDouble() > 0.5;
        leftmost = false;

        while (strSeed.Length < 9)
            strSeed = left ? '0' + strSeed : strSeed + '0';

        int offset = Rnd.Next(0, 10);

        for (int i = 0; i < wires.Length; i++)
            wires[i] = (int)(char.GetNumericValue(strSeed[leftmost ? i : i + (9 - wires.Length)]) + offset) % 10;

        char[] baseN = new char[Rnd.Next(20, 63)];

        for (int i = 0; i < baseN.Length; i++)
            baseN[i] = StaticArrays.Base62[i];

        _init.Seed = Algorithms.ConvertFromBase10(value: int.Parse(strSeed), baseChars: baseN);
        _init.RoleReversal.Texts[0].text = "Seed: " + _init.Seed;
        _init.RoleReversal.Texts[1].text = "Wire: " + _init.WireSelected;

        Debug.LogFormat("[Reformed Role Reversal #{0}]: Seed in Base {1}: {2} - Seed in Base 10: {3} - # of wires: {4}.", _init.ModuleId, baseN.Length, _init.Seed, strSeed, wires.Length);
        Debug.LogFormat("[Reformed Role Reversal #{0}]: Append 0's on the left: {1}, grab leftmost wires: {2}, using table {3}.", _init.ModuleId, left, leftmost, offset);

        string[] log = new string[wires.Length];

        for (int i = 0; i < wires.Length; i++)
            log[i] += i == wires.Length - 1 ? "and " + StaticArrays.Colors[wires[i]] : StaticArrays.Colors[wires[i]];

        Debug.LogFormat("[Reformed Role Reversal #{0}]: The wires are {1}.", _init.ModuleId, log.Join(", "));

        int i2 = _init.Conditions.GetLength(0), j2 = _init.Conditions.GetLength(1);

        _tutorial = new StaticArrays(_init.RoleReversal.Info).GetTutorial(_init.Interact.ButtonOrder, baseN.Length, ref left, ref leftmost, ref offset);

        for (int i = 0; i < i2; i++)
            for (int j = 0; j < j2; j++)
                _coroutines.GenerateCondition(i, j, wires, ref strSeed);
    }

    protected internal IEnumerator GenerateCondition(int i, int j, int[] wires, string strSeed)
    {
        yield return null;

        if (i == 0)
        {
            _init.Conditions[i, j] = _tutorial[j];

            yield return new WaitWhile(() => _init.Conditions[i, j] == null);
            _generateCondition++;

            yield break;
        }

        const string randomFirstAndLastMethods = "ABC";
        const string randomMethods = "ABCDEFGHIJKLMNOSTUVWXYZ";
        // P Q R
        //const string randomMethods = "J";

        Type classType = typeof(Manual);
        MethodInfo methodInfo;

        switch (j)
        {
            // First case. (Guaranteed edgework)
            case 0: methodInfo = classType.GetMethod("First" + randomFirstAndLastMethods[Rnd.Next(0, randomFirstAndLastMethods.Length)].ToString()); break;

            // Last case. (Guaranteed no edgework)
            case 7: methodInfo = classType.GetMethod("Last" + randomFirstAndLastMethods[Rnd.Next(0, randomFirstAndLastMethods.Length)].ToString()); break;

            // Every other case. (Mixed)
            default: methodInfo = classType.GetMethod(randomMethods[Rnd.Next(0, randomMethods.Length)].ToString()); break;
        }
        
        _init.Conditions[i, j] = (Condition)methodInfo.Invoke(this, new object[] { wires, _roleReversal.Info });

        yield return new WaitWhile(() => _init.Conditions[i, j] == null);
        _generateCondition++;

        if (_generateCondition == _init.Conditions.GetLength(0) * _init.Conditions.GetLength(1))
            _init.CorrectAnswer = GetAnswer(ref strSeed);
    }

    private int? GetAnswer(ref string strSeed)
    {
        _coroutines.UpdateScreen(instructionX: 0, instructionY: 0, wireSelected: 1);

        int wires = (int.Parse(strSeed) % 7) + 1, i2 = _init.Conditions.GetLength(1);

        for (int i = 0; i < i2; i++)
        {
            if (_init.Conditions[wires, i].SkipTo != null)
            {
                Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is true, skip to section {4}.", _init.ModuleId, wires + 2, i + 1, _init.Conditions[wires, i].Text, _init.Conditions[wires, i].SkipTo);
                Init.LightsOn = true;
                i = (int)_init.Conditions[wires, i].SkipTo - 1;
            }

            else if (_init.Conditions[wires, i].Wire != null)
            {
                Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is true, cut the {4} wire.", _init.ModuleId, wires + 2, i + 1, _init.Conditions[wires, i].Text, StaticArrays.Ordinals[(int)_init.Conditions[wires, i].Wire - 1]);
                Init.LightsOn = true;
                return (int)_init.Conditions[wires, i].Wire;
            }

            Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is false.", _init.ModuleId, wires + 2, i + 1, _init.Conditions[wires, i].Text);
        }
        
        Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> Unreachable code detected, please cut any wire to solve the module.", _init.ModuleId);
        Init.LightsOn = true;
        return null;
    }
}
