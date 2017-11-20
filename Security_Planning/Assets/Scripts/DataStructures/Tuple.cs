using System;
using UnityEngine;

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
        
        if (obj.GetType() == typeof(Tuple<T1,T2>))
        {
            Tuple<T1,T2> castedObj = (Tuple<T1, T2>)obj;
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
        return "(" + First.ToString() + ", " + Second.ToString() + ")";
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

