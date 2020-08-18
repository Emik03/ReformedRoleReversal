using System;
using System.Reflection;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;

internal class HandleManual
{
    internal HandleManual(Init init)
    {
        _init = init;
    }
    
    protected internal readonly int Seed = Rnd.Next(0, 1000000000);

    private readonly Init _init;
    private static readonly System.Random Rnd = new System.Random();

    /// <summary>
    /// Converts the random number generated into wires, and a seed for the module to display.
    /// </summary>
    protected internal void FormatSeed()
    {
        _init.LightsOn = true;
        _init.ModuleId = Init.ModuleIdCounter++;

        int[] wires = new int[(Seed % 7) + 3];

        string strSeed = Seed.ToString();

        while (strSeed.Length < 9)
            strSeed = '0' + strSeed;

        for (int i = 0; i < wires.Length; i++)
            wires[i] = (int)char.GetNumericValue(strSeed[i]);

        UnityEngine.Debug.Log(wires.Join(", "));

        _init.Seed = Algorithms.IntToString(int.Parse(strSeed),
            new char[] { '0','1','2','3','4','5','6','7','8','9',
            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'});

        GenerateManual(ref strSeed, wires);
    }

    /// <summary>
    /// Generates a manual with random conditions in random order.
    /// </summary>
    private void GenerateManual(ref string strSeed, int[] wires)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < _init.Conditions.GetLength(0); i++)
            for (int j = 0; j < _init.Conditions.GetLength(1); j++)
                _init.RoleReversal.GenerateCondition(i, j, wires);

        for (int i = 0; i < _init.Conditions.GetLength(1); i++)
            _init.Conditions[0, i] = StaticArrays.Tutorial[i];
        stopwatch.Stop();
        UnityEngine.Debug.Log("I took " + stopwatch.Elapsed + "ms to generate!");
        _init.CorrectAnswer = GetAnswer(ref strSeed);
        _init.RoleReversal.UpdateScreen(instructionX: 0, instructionY: 0, wireSelected: 1);
    }

    protected internal IEnumerator GenerateCondition(int i, int j, int[] wires)
    {
        yield return null;

        //const string randomMethods = "ABCDEFGWXYZ";
        const string randomMethods = "AA";
        int rng, previous = 0;
        Type classType = typeof(Manual);
        MethodInfo methodInfo;

        switch (j)
        {
            case 0: methodInfo = Rnd.NextDouble() > 0.5 ? classType.GetMethod("FirstA") : classType.GetMethod("FirstB"); break;

            case 6: methodInfo = Rnd.NextDouble() > 0.5 ? classType.GetMethod("LastA") : classType.GetMethod("LastB"); break;

            default:
                do rng = Rnd.Next(0, randomMethods.Length);
                while (rng == previous);

                previous = rng;
                methodInfo = classType.GetMethod(randomMethods[rng].ToString());
                break;
        }
        
        _init.Conditions[i, j] = (Condition)methodInfo.Invoke(this, new object[] { wires, _init.RoleReversal.Info });
    }

    protected internal void Cond(int i, int j, int[] wires)
    {
        //const string randomMethods = "ABCDEFGWXYZ";
        const string randomMethods = "AA";
        int rng, previous = 0;
        Type classType = typeof(Manual);
        MethodInfo methodInfo;

        switch (j)
        {
            case 0: methodInfo = Rnd.NextDouble() > 0.5 ? classType.GetMethod("FirstA") : classType.GetMethod("FirstB"); break;

            case 6: methodInfo = Rnd.NextDouble() > 0.5 ? classType.GetMethod("LastA") : classType.GetMethod("LastB"); break;

            default:
                do rng = Rnd.Next(0, randomMethods.Length);
                while (rng == previous);

                previous = rng;
                methodInfo = classType.GetMethod(randomMethods[rng].ToString());
                break;
        }

        _init.Conditions[i, j] = (Condition)methodInfo.Invoke(this, new object[] { wires, _init.RoleReversal.Info });
    }

    private int GetAnswer(ref string strSeed)
    {
        //for (int i = 0; i < _init.Conditions.GetLength(1); i++)
        //{
        //    if (_init.Conditions[(int.Parse(strSeed) % 7) + 1, i].Skip != 0)
        //        i = _init.Conditions[(int.Parse(strSeed) % 7) + 1, i].Skip - 2;

        //    else if (_init.Conditions[(int.Parse(strSeed) % 7) + 1, i].Wire != 0)
        //        return _init.Conditions[(int.Parse(strSeed) % 7) + 1, i].Wire;
        //}

        return 0;
    }
}
