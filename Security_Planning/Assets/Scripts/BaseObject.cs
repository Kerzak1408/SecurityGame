using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class BaseObject : MonoBehaviour
    {
        public virtual void StartGame() { }

        protected virtual void Start() { }

        protected virtual void Update ()
        {
        }

        protected IEnumerator CallAfterTimeout(float timeout, Action action)
        {
            while (timeout > 0)
            {
                timeout -= Time.deltaTime;
                yield return null;
            }
            action();
        }
    }
}
