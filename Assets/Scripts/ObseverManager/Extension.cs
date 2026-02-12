using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Extension
{
    #region CoreTemplate

    public static RectTransform rect(this Component c)
    {
        return c.GetComponent<RectTransform>();
    }

    public static string GetFullNameMethod(this Delegate d)
    {
        return $"{d.Method.DeclaringType}.{d.Method.Name}";
    }

    public static IEnumerable<T> GetValues<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    #endregion
}