using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class BaseObject : MonoBehaviour
    {
        private bool isInitialized;

        public virtual void StartGame()
        {
            isInitialized = true;
        }

        //protected virtual void Start() { }

        private void Update()
        {
            if (isInitialized)
            {
                UpdateGame();
            }
        }

        protected virtual void UpdateGame() { }
    }
}
