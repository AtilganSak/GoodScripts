using System;
using UnityEngine;

[System.Serializable]
public struct EasyNumber
{
    [NamedArray]
    [SerializeField] double[] steps;

    bool isCombined;

    private double _value;
    public double Value
    {
        get
        {
            if (!isCombined)
                Combine();

            return _value;
        }        
        set { _value = value; }
    }

    public override string ToString()
    {
        return Necessary.Convert(Value);
    }
    void Combine()
    {
        isCombined = true;
        if (steps != null && steps.Length > 0)
        {
            _value = 0;
            for (int i = 0; i < steps.Length; i++)
            {
                _value += steps[i] * Math.Pow(1000, i);
            }
            steps = null;
        }
    }
    public void Clear()
    {
        isCombined = false;
        steps = null;
        _value = 0;
    }
    #region Operators    
    public static EasyNumber operator +(EasyNumber a, double b)
    {
        a._value += b;
        return a;
    }
    public static EasyNumber operator -(EasyNumber a, double b)
    {
        a._value -= b;
        return a;
    }
    public static EasyNumber operator *(EasyNumber a, double b)
    {
        a._value *= b;
        return a;
    }
    public static EasyNumber operator /(EasyNumber a, double b)
    {
        a._value /= b;
        return a;
    }
    public static EasyNumber operator +(EasyNumber a, EasyNumber b)
    {
        a._value += b._value;
        return a;
    }
    public static EasyNumber operator -(EasyNumber a, EasyNumber b)
    {
        a._value -= b._value;
        return a;
    }
    public static EasyNumber operator *(EasyNumber a, EasyNumber b)
    {
        a._value *= b._value;
        return a;
    }
    public static EasyNumber operator /(EasyNumber a, EasyNumber b)
    {
        a._value /= b._value;
        return a;
    }
    #endregion
}