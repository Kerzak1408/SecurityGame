using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
    public class BaseSlidingGate : BaseGate
    {
        public bool horizontal;
        public GameObject gate;

        private bool isClosing;
        private Vector3 defaultPosition;
        private Vector3 lossyScale;
        private Vector3 translationDirection;
        private float speed;
        private Axis axis;

        protected virtual void Start()
        {
            isClosing = true;
            defaultPosition = gate.transform.position;
            lossyScale = gate.transform.lossyScale;
            speed = 4f;
            translationDirection = horizontal ? Vector3.right : Vector3.up;
            axis = horizontal ? Axis.X : Axis.Y;
        }

        protected override void Update()
        {
            base.Update();
            if (isClosing)
            {
                bool gateFullyClosed = !(gate.transform.position.GetVectorCoord(axis) > defaultPosition.GetVectorCoord(axis));

                if (!gateFullyClosed)
                {
                    gate.transform.position -= speed * translationDirection * Time.deltaTime;
                    if (gate.transform.position.GetVectorCoord(axis) < defaultPosition.GetVectorCoord(axis))
                    {
                        gate.transform.position = defaultPosition;
                    }
                }
            }
            else
            {
                bool gateFullyOpen = !(gate.transform.position.GetVectorCoord(axis) < defaultPosition.GetVectorCoord(axis) + lossyScale.GetVectorCoord(axis));

                if (!gateFullyOpen)
                {
                    gate.transform.position += speed * translationDirection * Time.deltaTime;
                }
                else if (lockAfterOpening)
                {
                    Lock();
                }
            }
        }

        public override void Open()
        {
            isClosing = false;
        }

        public override void Close()
        {
            isClosing = true;
        }
    }
}
