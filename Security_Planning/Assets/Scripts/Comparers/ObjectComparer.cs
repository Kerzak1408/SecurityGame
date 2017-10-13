using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectComparer : IComparer<UnityEngine.Object>
{
    public int Compare(UnityEngine.Object x, UnityEngine.Object y)
    {
        return string.Compare(x.name, y.name);
    }

}
