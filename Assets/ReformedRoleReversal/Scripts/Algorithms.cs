using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contains all algorithms for use in manual creation.
/// </summary>
static class Algorithms
{
    /// <summary>
    /// Finds and returns the index of the wires that match the method used.
    /// </summary>
    /// <param name="method">The method used to locate a match.</param>
    /// <param name="key">The key that is used as comparison for methods with the word 'Key'.</param>
    /// <param name="wires">The array to search with.</param>
    /// <returns>The index of the wire to cut.</returns>
    internal static int? Find(string method, ref int key, int[] wires)
    {
        switch (method)
        {
            case "firstInstanceOfKey":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] == key)
                        return ++i;
                break;

            case "firstInstanceOfNotKey":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] != key)
                        return ++i;
                break;

            case "lastInstanceOfKey":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] == key)
                        return ++i;
                break;

            case "lastInstanceOfNotKey":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] != key)
                        return ++i;
                break;

            case "firstInstanceOfOppositeKey":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] == (key + 5) % 10)
                        return ++i;
                break;

            case "lastInstanceOfOppositeKey":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] == (key + 5) % 10)
                        return ++i;
                break;

            case "firstInstanceOfBlue":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] < 5)
                        return ++i;
                break;

            case "firstInstanceOfPurple":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] >= 5)
                        return ++i;
                break;

            case "lastInstanceOfBlue":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] < 5)
                        return ++i;
                break;

            case "lastInstanceOfPurple":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] >= 5)
                        return ++i;
                break;

            case "lowestEven":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] % 2 == 1)
                        return ++i;
                break;

            case "highestEven":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] % 2 == 1)
                        return ++i;
                break;

            case "lowestOdd":
                for (int i = 0; i < wires.Length; i++)
                    if (wires[i] % 2 == 1)
                        return ++i;
                break;

            case "highestOdd":
                for (int i = wires.Length - 1; i >= 0; i--)
                    if (wires[i] % 2 == 1)
                        return ++i;
                break;

            default: throw new NotImplementedException("Could not find '" + method + "' for Algorithms.Find(), did you misspell the string?");
        }

        return null;
    }

    /// <summary>
    /// Returns the earliest index of an array that isn't null.
    /// </summary>
    /// <param name="array">The array used to locate the smallest number.</param>
    /// <returns>The smallest number that isn't null.</returns>
    internal static int? First(int?[] array)
    {
        int? min = 10;

        for (int i = 0; i < array.Length; i++)
            if (min > array[i] && array[i] != null)
                min = array[i];

        return min;
    }

    /// <summary>
    /// Returns the earliest index of a list that isn't 0.
    /// </summary>
    /// <param name="array">The array used to locate the smallest number.</param>
    /// <returns>The smallest number not equal to 0.</returns>
    internal static int First(List<int> array)
    {
        int min = 10;

        for (int i = 0; i < array.Count; i++)
            if (min > array[i] && array[i] != 0)
                min = array[i];

        return min;
    }

    internal static void RevertLookup(int[] wires, ref int lookup)
    {
        for (int i = 0; i < wires.Length; i++)
            wires[i] = (wires[i] - lookup + 10) % 10;
    }

    /// <summary>
    /// Returns the amount of colors for each group.
    /// </summary>
    /// <param name="grouped">Whether or not to return blueish against purpleish, or every color individually.</param>
    /// <param name="wires">The array to search with.</param>
    /// <returns>The list of colors, with size 2 or 10 depending on the 'grouped' boolean.</returns>
    internal static int[] GetColors(bool grouped, int[] wires)
    {
        int[] colors = grouped ? new int[2] { 0, 0 } : new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        if (grouped)
            for (int i = 0; i < wires.Length; i++)
                colors[wires[i] / 5]++;

        else
            for (int i = 0; i < wires.Length; i++)
                colors[wires[i]]++;

        return colors;
    }

    /// <summary>
    /// Returns an array of random unique numbers with a few parameters.
    /// </summary>
    /// <param name="length">The length of the array.</param>
    /// <param name="min">The included minimum value.</param>
    /// <param name="max">The excluded maximum value.</param>
    /// <returns>A random integer array.</returns>
    internal static int[] Random(int length, int min, int max)
    {
        int[] range = Enumerable.Range(min, --max).ToArray().Shuffle(), array = new int[length];

        if (range.Length < length)
            throw new ArgumentOutOfRangeException("range: " + range.Join(", "), "The length of the returned array (" + length + ") is larger than the range specified (" + range.Length + ")!");

        for (int i = 0; i < array.Length; i++)
            array[i] = range[i];

        return array;
    }

    /// <summary>
    /// An optimized method using an array as buffer instead of string concatenation. 
    /// This is faster for return values having a length > 1.
    /// </summary>
    public static string ConvertFromBase10(int value, char[] baseChars)
    {
        // 32 is the worst cast buffer size for base 2 and int.MaxValue
        int i = 32;
        char[] buffer = new char[i];
        int targetBase = baseChars.Length;

        do
        {
            buffer[--i] = baseChars[value % targetBase];
            value = value / targetBase;
        }
        while (value > 0);

        char[] result = new char[32 - i];
        Array.Copy(buffer, i, result, 0, 32 - i);

        return new string(result);
    }

    /// <summary>
    /// Adds vertical bar characters which are placeholders for line breaks to the submitted string.
    /// </summary>
    /// <param name="text">The text to add line breaks with.</param>
    /// <returns>A modified string containing vertical bars.</returns>
    internal static string LineBreaks(string text)
    {
        const byte jump = 27;
        ushort index = jump;
        StringBuilder sb = new StringBuilder(text);

        while (index < text.Length)
        {
            if (text[index] == ' ')
            {
                sb[index] = '\n';
                index += jump;
            }

            else
                index--;
        }

        return sb.ToString();
    }
}
