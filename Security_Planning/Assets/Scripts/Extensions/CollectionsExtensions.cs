using System.Collections.Generic;
using UnityEngine;

public static class CollectionsExtensions
{
    public static Tuple<int, int> GetIndices<T>(this T[,] array, T item)
    {
        for (int i = 0; i < array.GetLength(0); i++)
            for (int j = 0; j < array.GetLength(1); j++)
            {
                if (array[i,j].Equals(item))
                {
                    return Tuple.New(i, j);
                }
            }
        return null;
    }

    public static string ToStringExtended<T>(this T[] array)
    {
        if (array.Length == 0)
        {
            return "[]";
        }
        var result = "[";
        for (int i = 0; i < array.Length - 1; i++)
        {
            var current = array[i];
            result += current.ToString() + ", ";
        }
        result += array[array.Length - 1] +  "]";
        return result;
    }

    public static bool Contains<T>(this T[,] array, T seekedItem)
    {
        foreach (T item in array)
        {
            if (item.Equals(seekedItem))
            {
                return true;
            }
        }
        return false;
    }

    public static T Get<T>(this T[,] array, Tuple<int, int> indices)
    {
        return array[indices.First, indices.Second];
    }

    public static Tuple<int, int> MultiplyBy(this Tuple<int, int> tuple, int n)
    {
        return Tuple.New(tuple.First * n, tuple.Second * n);
    }

    public static List<T> Copy<T>(this List<T> original)
    {
        var copy = new List<T>();
        foreach (T item in original)
        {
            copy.Add(item);
        }

        return copy;
    }

    /// <summary>
    /// Add to the dictionary where values is a collection. In case there is no such key yet,
    /// instantiate the collection first.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TCollection"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void LazyAdd<TKey, TCollection, TValue>(this Dictionary<TKey, TCollection> dictionary, 
        TKey key, TValue value)
        where TCollection : ICollection<TValue>, new()
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary[key] = new TCollection();
        }
        dictionary[key].Add(value);
    }
}
