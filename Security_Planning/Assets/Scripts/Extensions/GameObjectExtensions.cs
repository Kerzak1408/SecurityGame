using System;
using UnityEngine;
using UnityEngine.UI;

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

        public static void DeactivateAllCamerasAndAudioListeners(this GameObject gameObject)
        {
            gameObject.DeactivateComponentsOfTypeRecursively<Camera>();
            gameObject.DeactivateComponentsOfTypeRecursively<AudioListener>();
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

        public static void ChangeAlbedoAndEmissionColor(this GameObject gameObject, Color color)
        {
            gameObject.ChangeColors(color, color);
        }

        public static void ChangeColors(this GameObject gameObject, Color color, Color emissionColor)
        {
            var material = gameObject.GetComponent<Renderer>().material;
            material.color = color;
            material.SetColor("_EmissionColor", emissionColor);
        }

        public static void ChangeMaterialAndColor(this GameObject gameObject, Color color)
        {
            var material = gameObject.GetComponent<Renderer>().material;
            var materialCopy = new Material(material);
            materialCopy.color = color;
            gameObject.GetComponent<Renderer>().material = materialCopy;
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

        /// <summary>
        /// Returns the component of type <paramref name="T"/> of the <paramref name="gameObject"/> if there is such component,
        /// otherwise return this kind of component from the children of <paramref name="gameObject"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static T GetComponentInGameObjectOrChildren<T>(this GameObject gameObject)
        {
            var result = gameObject.GetComponent<T>();
            if (result != null)
            {
                return result;
            }
            result = gameObject.GetComponentInChildren<T>();
            return result;
        }

        /// <summary>
        /// Change dropdown value cyclically up or down according to <paramref name="down"/> value.
        /// </summary>
        /// <param name="dropdown"></param>
        /// <param name="down"></param>
        public static void ChangeDropdownValue(this Dropdown dropdown, bool down=true)
        {
            int optionsCount = dropdown.options.Count;
            if (down)
            {
                dropdown.value = (dropdown.value + 1) % optionsCount;
            }
            else
            {
                int decreasedValue = dropdown.value - 1;
                dropdown.value = decreasedValue < 0 ? optionsCount - 1 : decreasedValue;
            }
            dropdown.RefreshShownValue();
        }
    }
}
