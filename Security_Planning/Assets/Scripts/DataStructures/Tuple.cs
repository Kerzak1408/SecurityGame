using System;
using UnityEngine;

//namespace Assets.Scripts.DataStructures
//{
    /// <summary>
    /// Unity does not support C# Tuple yet, so this is the replacement for it.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    [Serializable]
    public class Tuple<T1, T2>
    {
        [SerializeField]
        public T1 First { get; private set; }
        [SerializeField]
        public T2 Second { get; private set; }

        internal Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override bool Equals(object obj)
        {
            var castedObj = obj as Tuple<T1, T2>;
            if (castedObj != null)
            {
                var result = First.Equals(castedObj.First) && Second.Equals(castedObj.Second);
                return result;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() + Second.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + First + ", " + Second + ")";
        }

    }

    public static class Tuple
    {
        public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
        {
            var tuple = new Tuple<T1, T2>(first, second);
            return tuple;
        }
    }
//}