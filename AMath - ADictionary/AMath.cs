using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AMath
{
    /// <summary>
    /// "Returns the positive value"
    /// </summary>
    public static int BeNegative(int val)
    {
        if (val < 0)
            return -val;
        else
            return val;
    }
    /// <summary>
    /// "Returns the positive value"
    /// </summary>
    public static float BeNegative(float val)
    {
        if (val > 0)
            return -val;
        else
            return val;
    }
    /// <summary>
    /// "Returns the positive value"
    /// </summary>
    public static int BePositive(int val)
    {
        if (val < 0)
            return -val;
        else
            return val;
    }
    /// <summary>
    /// "Returns the positive value"
    /// </summary>
    public static float BePositive(float val)
    {
        if (val < 0)
            return -val;
        else
            return val;
    }
    /// <summary>
    /// "Returns the largest float, mid float(0.5F, 1.5F,...) or equal to f."
    /// </summary>
    public static float RollFloat(float val)
    {
        float firstNumber = Mathf.Floor(val);
        float a = firstNumber + 1;
        float b = firstNumber + 0.5F;
        float c = firstNumber;

        float d = a - val;
        float e = b - val;
        float f = c - val;

        if (d < 0) d = -d;
        if (e < 0) e = -e;
        if (f < 0) f = -f;

        if (d < e && d < f) return a;
        else if (e < d && e < f) return b;
        else return c;
    }
}
