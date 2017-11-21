﻿using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
    public abstract class BaseGate : MonoBehaviour
    {
        private int noMovementsTurn;
        protected bool lockAfterOpening;
        public bool Locked { get; private set; }

        public abstract void Open();
        public abstract void Close();

        public virtual void Lock()
        {
            Locked = true;
        }

        public virtual void Unlock()
        {
            AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Sounds/KeyLock"), transform.position);
            Locked = false;
        }

        public void UnlockOnce()
        {
            Unlock();
            lockAfterOpening = true;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.transform.gameObject.HasScriptOfType<BaseCharacter>()) return;
            Debug.Log("OnTriggerEnter");
            noMovementsTurn = 0;
            if (!Locked)
            {
                Open();
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (!other.transform.gameObject.HasScriptOfType<BaseCharacter>()) return;
            Debug.Log("OnTriggerStay");
            noMovementsTurn = 0;
            if (!Locked)
            {
                Open();
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.transform.gameObject.HasScriptOfType<BaseCharacter>()) return;
            Debug.Log("OnTriggerExit");
            noMovementsTurn = 0;
        }

        protected virtual void Update()
        {
            if (noMovementsTurn > 20)
            {
                Close();
            }
            noMovementsTurn++;
        }
    }
}
