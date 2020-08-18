using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contains all algorithms for use in manual creation.
/// </summary>
static class Algorithms
{
    private static readonly Random Rnd = new Random();

    /// <summary>
    /// Finds and returns the index of the wires that match the method used.
    /// </summary>
    /// <param name="method">The method used to locate a match.</param>
    /// <param name="key">The key that is used as comparison for methods with the word 'Key'.</param>
    /// <param name="wires">The array to search with.</param>
    /// <returns>The index of the wire to cut.</returns>
    internal static int Find(string method, ref int key, int[] wires)
    {
        switch (method)
        {
            case "firstInstanceOfKey":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] == key)
                        return i + 1;
                break;

            case "firstInstanceOfNotKey":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] != key)
                        return i + 1;
                break;

            case "lastInstanceOfKey":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] == key)
                        return i + 1;
                break;

            case "lastInstanceOfNotKey":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] != key)
                        return i + 1;
                break;

            case "firstInstanceOfBlue":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] < 5)
                        return i + 1;
                break;

            case "firstInstanceOfPurple":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] >= 5)
                        return i + 1;
                break;

            case "lastInstanceOfBlue":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] < 5)
                        return i + 1;
                break;

            case "lastInstanceOfPurple":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] >= 5)
                        return i + 1;
                break;

            default:
                throw new NotImplementedException();
        }

        return 0;
    }

    /// <summary>
    /// Returns the earliest index of an array that isn't 0.
    /// </summary>
    /// <param name="array">The array used to locate the smallest number.</param>
    /// <returns>The smallest number not equal to 0.</returns>
    internal static int EarliestIndex(int[] array)
    {
        int min = 10;

        for (int i = 0; i < array.Length; i++)
            if (min > array[i] && array[i] != 0)
                min = array[i];

        return min;
    }

    /// <summary>
    /// Returns the earliest index of an array that isn't 0.
    /// </summary>
    /// <param name="array">The array used to locate the smallest number.</param>
    /// <returns>The smallest number not equal to 0.</returns>
    internal static int EarliestIndex(List<int> array)
    {
        int min = 10;

        for (int i = 0; i < array.Count; i++)
            if (min > array[i] && array[i] != 0)
                min = array[i];

        return min;
    }

    /// <summary>
    /// Returns an array of random numbers with a few parameters.
    /// </summary>
    /// <param name="length">The length of the array.</param>
    /// <param name="minValue">The included minimum value.</param>
    /// <param name="maxValue">The excluded maximum value.</param>
    /// <returns>A random integer array.</returns>
    internal static int[] Randomize(int length, int minValue, int maxValue)
    {
        int[] parameters = new int[length];
        for (int i = 0; i < length; i += 0)
        {
            int random = Rnd.Next(minValue, maxValue);

            if (!parameters.Contains(random))
            {
                parameters[i] = random;
                i++;
            }
        }
        return parameters;
    }

    /// <summary>
    /// Adds vertical bar characters which are placeholders for line breaks to the submitted string.
    /// </summary>
    /// <param name="text">The text to add line breaks with.</param>
    /// <returns>A modified string containing vertical bars.</returns>
    internal static string AddLineBreakPlaceholders(string text)
    {
        ushort index = 33;
        StringBuilder sb = new StringBuilder(text);

        while (index < text.Length)
        {
            if (text[index] == ' ')
            {
                sb[index] = '|';
                index += 33;
            }

            else
                index--;
        }

        return sb.ToString();
    }
}
