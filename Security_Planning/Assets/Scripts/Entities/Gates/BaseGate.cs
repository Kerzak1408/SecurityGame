using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
    public abstract class BaseGate : MonoBehaviour
    {
        private bool anyMovement;
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
            Locked = false;
        }

        public void UnlockOnce()
        {
            Unlock();
            lockAfterOpening = true;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            anyMovement = true;
            if (!Locked)
            {
                Open();
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            anyMovement = true;
            if (!Locked)
            {
                Open();
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            anyMovement = true;
        }

        protected virtual void Update()
        {
            if (!anyMovement)
            {
                Close();
            }
            anyMovement = false;
        }
    }
}
