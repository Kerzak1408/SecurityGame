using System;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Entities
{
    public class CardReaderEntity : TransmitterEntity
    {
        private GateOpen relatedGate;

        public override Type GetReceiverType()
        {
            return typeof(GateOpen);
        }

        public override void StartGame()
        {
            relatedGate = GameObject.Find(Data.relatedName).GetComponent<GateOpen>();
            relatedGate.Close();
            relatedGate.Lock();
        }

        public void VerifyCard()
        {
            // TODO: verify card
            relatedGate.UnlockOnce();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.HasScriptOfType<BaseCharacter>())
            {
                BaseCharacter baseCharacter = other.gameObject.GetComponent<BaseCharacter>();
                baseCharacter.RequestCard(this);
            }
        }
    }
}
