using System;
using UnityEngine;

namespace Assets.Scripts.Extensions
{
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
            gameObject.DeactivateComponentsOfTypeRecursively<MonoBehaviour>();
        }

        public static void DeactivateAllCameras(this GameObject gameObject)
        {
            gameObject.DeactivateComponentsOfTypeRecursively<Camera>();
        }

        public static void DeactivateComponentsOfTypeRecursively<T>(this GameObject gameObject) where T : Behaviour
        {
            gameObject.DeactivateComponentsOfType<T>();
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.DeactivateComponentsOfTypeRecursively<T>();
            }
        }

        public static void DeactivateComponentsOfType<T>(this GameObject gameObject) where T : Behaviour
        {
            var components = gameObject.GetComponents<T>();
            if (components != null)
            {
                foreach (var component in components)
                {
                    component.enabled = false;
                }
            }
        }

        public static bool HasScriptOfType<T>(this GameObject gameObject) where T : MonoBehaviour
        {
            var potentialScript = gameObject.GetComponent<T>();
            return potentialScript != null;
        }

        public static bool HasScriptOfType(this GameObject gameObject, Type type)
        {
            var potentialScript = gameObject.GetComponent(type);
            return potentialScript != null;
        }

        public static void ChangeColor(this GameObject gameObject, Color color)
        {
            var material = gameObject.GetComponent<Renderer>().material;
            material.color = color;
            material.SetColor("_EmissionColor", color);
        }

        public static bool IsChildOf(this GameObject potentialChild, GameObject potentialParent)
        {
            foreach (Transform transform in potentialParent.transform)
            {
                if (transform.gameObject.Equals(potentialChild))
                {
                    return true;
                }
            }
            return false;
        }

        /// <param name="clipName"> The clip is supposed to be located in Assets/Resources/Sounds </param>
        public static AudioSource AttachAudioSource(this GameObject gameObject, string clipName, float pitch = 1)
        {
            AudioSource result = gameObject.AddComponent<AudioSource>();
            result.spatialBlend = 1;
            result.rolloffMode = AudioRolloffMode.Linear;
            result.playOnAwake = false;
            result.clip = Resources.Load<AudioClip>("Sounds/" + clipName);
            result.minDistance = 0.2f;
            result.maxDistance = 5;

            result.pitch = pitch;

            return result;
        }
    }
}
