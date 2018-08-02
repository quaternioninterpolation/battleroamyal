// // --
// // Author: Josh van den Heever
// // Date: 1/08/2018 @ 5:02 p.m.
// // --
using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public static class ExtensionMethods
{
    public static T[] GetListOfStaticPublicMemberValues<T>(this Type classType)
    {
        return classType
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where((arg) => arg.FieldType == typeof(T))
            .Select((arg) => (T)arg.GetValue(null))
            .ToArray();
    }

    public static T GetRandom<T>(this T[] array)
    {
        if (array.Length == 0) return default(T);
        return array[UnityEngine.Random.Range(0, array.Length-1)];
    }

    public static Vector3 ToVector3(this Vector2 input)
    {
        return new Vector3(input.x, input.y, 0f);
    }

    public static Vector2 ToVector2(this Vector3 input)
    {
        return new Vector2(input.x, input.y);
    }
}
