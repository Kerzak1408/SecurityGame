using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
    public class BaseSlidingGate : BaseGate
    {
        public Axis Direction;
        public GameObject gate;

        private bool isClosing;
        private Vector3 defaultPosition;
        private Vector3 translationDirection;
        private float speed;
        private AudioSource slidingDoorOpen;
        private AudioSource slidingDoorClose;
        private double DOOR_SIZE = 1.1;

        protected bool IsGateFullyOpen
        {
            get
            {
                return !(gate.transform.position.GetVectorCoord(Direction) <
                         defaultPosition.GetVectorCoord(Direction) + DOOR_SIZE);
            }
        }

        protected override void Start()
        {
            base.Start();
            isClosing = true;
            defaultPosition = gate.transform.position;
            speed = 2f;
            translationDirection = Direction.GetDirectionVector();
            slidingDoorClose = gameObject.AttachAudioSource("SlidingDoorClose");
            slidingDoorOpen = gameObject.AttachAudioSource("SlidingDoorOpen");
            if (Direction == Axis.Y) DOOR_SIZE = 2.2;
            else DOOR_SIZE = 1.1;
        }

        protected override void Update()
        {
            base.Update();
            if (isClosing)
            {
                bool gateFullyClosed = !(gate.transform.position.GetVectorCoord(Direction) > defaultPosition.GetVectorCoord(Direction));

                if (!gateFullyClosed)
                {
                    gate.transform.position -= speed * translationDirection * Time.deltaTime;
                    if (gate.transform.position.GetVectorCoord(Direction) < defaultPosition.GetVectorCoord(Direction))
                    {
                        gate.transform.position = defaultPosition;
                    }
                }
            }
            else
            {

                if (!IsGateFullyOpen)
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
