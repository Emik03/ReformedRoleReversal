using System;
using System.Reflection;
using UnityEngine;
using System.Collections;

internal class HandleManual
{
    internal HandleManual(Coroutines coroutines, Init init)
    {
        _coroutines = coroutines;
        _init = init;
        _roleReversal = init.RoleReversal;
    }
    
    protected internal readonly int Seed = Rnd.Next(0, 1000000000);

    private readonly Coroutines _coroutines;
    private readonly Init _init;
    private readonly ReformedRoleReversal _roleReversal;

    private static readonly System.Random Rnd = new System.Random();
    private int _generateCondition = 0;

    /// <summary>
    /// Converts the random number generated into wires, and a seed for the module to display.
    /// </summary>
    protected internal void Generate()
    {
        _init.LightsOn = true;
        _init.ModuleId = Init.ModuleIdCounter++;

        int[] wires = new int[(Seed % 7) + 3];

        string strSeed = Seed.ToString();

        while (strSeed.Length < 9)
            strSeed = '0' + strSeed;

        for (int i = 0; i < wires.Length; i++)
            wires[i] = (int)char.GetNumericValue(strSeed[i]);

        _init.Seed = Algorithms.Base10ToBaseN(int.Parse(strSeed), StaticArrays.Base62);

        Debug.LogFormat("[Reformed Role Reversal #{0}]: Seed: {1} - Base 10: {2} - # of wires: {3}.", _init.ModuleId, _init.Seed, strSeed, wires.Length);

        string[] log = new string[wires.Length];

        for (int i = 0; i < wires.Length; i++)
            log[i] += i == wires.Length - 1 ? "and " + StaticArrays.Colors[wires[i]] : StaticArrays.Colors[wires[i]];

        Debug.LogFormat("[Reformed Role Reversal #{0}]: The wires are {1}.", _init.ModuleId, log.Join(", "));
        
        for (int i = 0; i < _init.Conditions.GetLength(0); i++)
            for (int j = 0; j < _init.Conditions.GetLength(1); j++)
                _coroutines.GenerateCondition(i, j, wires, ref strSeed);
    }

    protected internal IEnumerator GenerateCondition(int i, int j, int[] wires, string strSeed)
    {
        yield return null;

        if (i == 0)
        {
            _init.Conditions[i, j] = StaticArrays.Tutorial[j];

            yield return new WaitWhile(() => _init.Conditions[i, j] == null);
            _generateCondition++;

            yield break;
        }

        const string randomMethods = "ABCDEFGWXYZ";
        //const string randomMethods = "CC";
        int rng, previous = 0;

        Type classType = typeof(Manual);
        MethodInfo methodInfo;

        switch (j)
        {
            // First case. (Guaranteed edgework)
            case 0: methodInfo = Rnd.NextDouble() > 0.5 ? classType.GetMethod("FirstA") : classType.GetMethod("FirstB"); break;

            // Last case. (Guaranteed no edgework)
            case 7: methodInfo = Rnd.NextDouble() > 0.5 ? classType.GetMethod("LastA") : classType.GetMethod("LastB"); break;

            // Every other case. (Mixed)
            default:
                do rng = Rnd.Next(0, randomMethods.Length);
                while (rng == previous);

                previous = rng;
                methodInfo = classType.GetMethod(randomMethods[rng].ToString());
                break;
        }
        
        _init.Conditions[i, j] = (Condition)methodInfo.Invoke(this, new object[] { wires, _init.RoleReversal.Info });

        yield return new WaitWhile(() => _init.Conditions[i, j] == null);
        _generateCondition++;

        if (_generateCondition == _init.Conditions.GetLength(0) * _init.Conditions.GetLength(1))
            _init.CorrectAnswer = GetAnswer(ref strSeed);
    }

    private int? GetAnswer(ref string strSeed)
    {
        _coroutines.UpdateScreen(instructionX: 0, instructionY: 0, wireSelected: 1);

        int wires = (int.Parse(strSeed) % 7) + 1;

        for (int i = 0; i < _init.Conditions.GetLength(1); i++)
        {
            if (_init.Conditions[wires, i].SkipTo != null)
            {
                Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is true, skip to section {4}.", _init.ModuleId, wires + 2, i + 1, _init.Conditions[wires, i].Text, _init.Conditions[wires, i].SkipTo);
                i = (int)_init.Conditions[wires, i].SkipTo - 1;
            }

            else if (_init.Conditions[wires, i].Wire != null)
            {
                Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is true, cut the {4} wire.", _init.ModuleId, wires + 2, i + 1, _init.Conditions[wires, i].Text, StaticArrays.Ordinals[(int)_init.Conditions[wires, i].Wire - 1]);
                Debug.Log(_init.Conditions[wires, i].Wire);
                return (int)_init.Conditions[wires, i].Wire;
            }

            Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> \"{3}\" is false.", _init.ModuleId, wires + 2, i + 1, _init.Conditions[wires, i].Text);
        }
        
        Debug.LogFormat("[Reformed Role Reversal #{0}]: <Condition {1}, {2}> Unreachable code detected, please cut any wire to solve the module.", _init.ModuleId);
        return null;
    }
}
