using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtensions {

    public static Object FindByName(this Object[] array, string name)
    {
        foreach (Object item in array)
        {
            if (item.name == name)
            {
                return item;
            }
        }
        return null;
    }
}
