using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions {

    public static bool IsEqualToChildOf(this GameObject gameObject, GameObject anotherGameObject)
    {
        foreach (Transform transform in anotherGameObject.transform)
        {
            if (transform.gameObject == gameObject)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsEqualToDescendantOf(this GameObject gameObject, GameObject anotherGameObject)
    {
        foreach (Transform transform in anotherGameObject.transform)
        {
            if (transform.gameObject == gameObject)
            {
                return true;
            }
            if (gameObject.IsEqualToDescendantOf(transform.gameObject))
            {
                return true;
            }
        }
        return false;
    }
}
