using System;
using System.Reflection;
using UnityEngine;

internal class HandleManual
{
    internal HandleManual(Init init)
    {
        Init = init;
    }

    protected internal Condition[,] Conditions = new Condition[7, 7];
    protected internal int Seed = Rnd.Next(0, 1000000000);

    private readonly Init Init;
    private static readonly System.Random Rnd = new System.Random();
    private int[] _wires;

    protected internal void GenerateSeed()
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

    private void GenerateManual()
    {
        Type classType = typeof(Manual);
        const string randomMethods = "ABCDWXYZ";
        int rng, previous = 0;
        
        for (int i = 0; i < Conditions.GetLength(0); i++)
            for (int j = 0; j < Conditions.GetLength(1); j++)
            {
                MethodInfo methodInfo;

                switch (j)
                {
                    case 0:
                        methodInfo = classType.GetMethod("First");
                        break;

                    case 6:
                        methodInfo = classType.GetMethod("Last");
                        break;

                    default:
                        do rng = Rnd.Next(0, randomMethods.Length);
                        while (rng == previous);

                        previous = rng;
                        methodInfo = classType.GetMethod(randomMethods[rng].ToString());

                        //Debug.Log(randomMethods[rng]);
                        break;
                }
                
                Conditions[i, j] = (Condition)methodInfo.Invoke(this, new object[] { _wires, Init.RoleReversal.Info });
                Debug.Log(Conditions[i, j].Text + " Correct Answer: " + Conditions[i, j].Wire + ", Go To Condition: " + Conditions[i, j].Skip);
            }
    }
}
