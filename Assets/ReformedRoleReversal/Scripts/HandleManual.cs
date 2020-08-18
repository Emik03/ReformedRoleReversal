using System;
using System.Reflection;
using UnityEngine;

internal class HandleManual
{
    internal HandleManual(Init init)
    {
        Init = init;
    }



    protected internal readonly int Seed = Rnd.Next(0, 1000000000);

    private readonly Init Init;
    private static readonly System.Random Rnd = new System.Random();
    private int[] _wires;

    /// <summary>
    /// Converts the random number generated into wires, and a seed for the module to display.
    /// </summary>
    protected internal void FormatSeed()
    {
        _wires = new int[(Seed % 7) + 3];

        string strSeed = Seed.ToString();

        while (strSeed.Length < 9)
            strSeed = '0' + strSeed;

        for (int i = 0; i < _wires.Length; i++)
            _wires[i] = (int)char.GetNumericValue(strSeed[i]);

        Debug.Log(_wires.Join(", "));

        GenerateManual();
    }

    /// <summary>
    /// Generates a manual with random conditions in random order.
    /// </summary>
    private void GenerateManual()
    {
        Type classType = typeof(Manual);
        //const string randomMethods = "ABCDEFGWXYZ";
        const string randomMethods = "GG";
        int rng, previous = 0;

        for (int i = 0; i < Init.Conditions.GetLength(1); i++)
            Init.Conditions[0, i] = StaticArrays.Tutorial[i];

        for (int i = 1; i < Init.Conditions.GetLength(0); i++)
            for (int j = 0; j < Init.Conditions.GetLength(1); j++)
            {
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
                
                Init.Conditions[i, j] = (Condition)methodInfo.Invoke(this, new object[] { _wires, Init.RoleReversal.Info });
                Debug.Log(Init.Conditions[i, j].Text + " Correct Answer: " + Init.Conditions[i, j].Wire + ", Go To Condition: " + Init.Conditions[i, j].Skip);
            }
    }
}
