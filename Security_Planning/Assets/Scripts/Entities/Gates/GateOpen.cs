using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
    public class GateOpen : BaseSlidingGate
    {
        protected virtual void OnTriggerEnter(Collider other)
        {
            Open();
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            Close();
        }
    }
}
