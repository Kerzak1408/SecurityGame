using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
    public abstract class BaseGate : MonoBehaviour
    {
        private float noMovementTime;
        protected bool lockAfterOpening;
        public bool Locked { get; private set; }

        public abstract void Open();
        public abstract void Close();

        private AudioSource keyLock;

        protected virtual void Start()
        {
            keyLock = transform.gameObject.AttachAudioSource("KeyLock");
        }

        protected virtual void Update()
        {
            if (noMovementTime > 1.5f)
            {
                Close();
            }
            noMovementTime += Time.deltaTime;
        }

        public virtual void Lock()
        {
            Locked = true;
        }

        public virtual void Unlock()
        {
            keyLock.Play();
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
            noMovementTime = 0;
            if (!Locked)
            {
                Open();
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (!other.transform.gameObject.HasScriptOfType<BaseCharacter>()) return;
            noMovementTime = 0;
            if (!Locked)
            {
                Open();
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.transform.gameObject.HasScriptOfType<BaseCharacter>()) return;
            noMovementTime = 0;
        }

    }
}
