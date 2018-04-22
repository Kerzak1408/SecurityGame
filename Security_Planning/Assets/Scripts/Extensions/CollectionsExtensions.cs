using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Extensions
{
    public static class CollectionsExtensions
    {
        /// <summary>
        /// Return the position of the <paramref name="item"/> in the <paramref name="array"/> or null
        /// if <paramref name="item"/> is not in <paramref name="array"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="item"></param>
        /// <returns></returns>
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

        /// <summary>
        /// It is not able to override array ToString() so this one is implemented for debugging.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
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
                result += current + ", ";
            }
            result += array[array.Length - 1] +  "]";
            return result;
        }

        /// <summary>
        /// Checks if <paramref name="array"/> contains <paramref name="seekedItem"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="seekedItem"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the <paramref name="T"/> item at the indices array[indices.First, indices.Second].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public static T Get<T>(this T[,] array, Tuple<int, int> indices)
        {
            return array[indices.First, indices.Second];
        }

        /// <summary>
        /// Shallow copy of the list. Creates a new instance of list but copies only references of the items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
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
        /// Shallow copy of the dictionary. Creates a new instance of dictionary but copies only references of the items.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Copy<TKey, TValue>(this Dictionary<TKey, TValue> original)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> keyValuePair in original)
            {
                result[keyValuePair.Key] = keyValuePair.Value;
            }
            return result;
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
}
