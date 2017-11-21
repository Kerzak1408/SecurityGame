using Assets.Scripts.Extensions;
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
        private AudioSource slidingDoorOpen;
        private AudioSource slidingDoorClose;

        protected override void Start()
        {
            base.Start();
            isClosing = true;
            defaultPosition = gate.transform.position;
            lossyScale = gate.transform.lossyScale;
            speed = 4f;
            translationDirection = horizontal ? Vector3.right : Vector3.up;
            axis = horizontal ? Axis.X : Axis.Y;
            slidingDoorClose = gameObject.AttachAudioSource("SlidingDoorClose");
            slidingDoorOpen = gameObject.AttachAudioSource("SlidingDoorOpen");
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
            if (!isClosing) return;
            isClosing = false;
            slidingDoorOpen.Play();
        }

        public override void Close()
        {
            if (isClosing) return;
            isClosing = true;
            slidingDoorClose.Play();
        }
    }
}
