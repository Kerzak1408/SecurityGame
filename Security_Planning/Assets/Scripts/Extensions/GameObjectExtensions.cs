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

    public static void DeactivateAllScripts(this GameObject gameObject)
    {
        var scripts = gameObject.GetComponents<MonoBehaviour>();
        if (scripts != null)
        {
            foreach (var script in scripts)
            {
                script.enabled = false;
            }
        }
    }

    public static bool HasScriptOfType<T>(this GameObject gameObject) where T : MonoBehaviour
    {
        var potentialScript = gameObject.GetComponent<T>();
        return potentialScript != null;
    }

    public static void ChangeColor(this GameObject gameObject, Color color)
    {
        var material = gameObject.GetComponent<Renderer>().material;
        material.color = color;
        material.SetColor("_EmissionColor", color);
    }
}
