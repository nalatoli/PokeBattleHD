using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtension
{
    /// <summary>
    /// Checks if two floats are only different by a certain error.
    /// </summary>
    /// <param name="a"> First float to compare. </param>
    /// <param name="value"> Second float to compare. </param>
    /// <param name="error"> Maximum difference between the the two floats. </param>
    /// <returns></returns>
    public static bool CloseTo(this float a, float value, float error)
    {
        error = Mathf.Abs(error);

        if (a < value)
            return (a + error) >= value;
        
        return (a - error) <= value;
    }

    /// <summary> Checks if float is between two numbers (inclusive). </summary>
    /// <param name="f"> Float to check. </param>
    /// <param name="a"> First limit. </param>
    /// <param name="b"> Second limit. </param>
    /// <returns> True is float is within values. </returns>
    public static bool IsBetween(this float f, float a, float b)
    {
        if(a < b)
            return f >= a && f <= b;

        else
            return f >= b && f <= a;

    }
}
