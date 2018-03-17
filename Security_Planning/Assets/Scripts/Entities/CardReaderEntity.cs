using System;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public class CardReaderEntity : TransmitterEntity, IInteractable
    {
        private GateOpen relatedGate;

        public override Type GetReceiverType()
        {
            return typeof(GateOpen);
        }

        public override void StartGame()
        {
            GameObject relateGateObject = GameObject.Find(Data.relatedName);
            if (relateGateObject != null)
            {
                relatedGate = relateGateObject.GetComponent<GateOpen>();
                relatedGate.Lock();
            }
            transform.position += Vector3.back;
        }

        public void VerifyCard()
        {
            // TODO: verify card
            if (relatedGate != null)
            {
                relatedGate.UnlockOnce();
            }
                
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.HasScriptOfType<CardItem>())
            {
                if (relatedGate != null)
                {
                    relatedGate.UnlockOnce();
                }
                other.gameObject.GetComponent<CardItem>().ResetItemPosition();
            }
        }

        public void Interact(BaseCharacter character, Action successAction = null)
        {
            GameObject activeItem = character.GetActiveItem();
            if (Vector3.Distance(transform.position, character.transform.position) < Constants.Constants.INTERACTABLE_DISTANCE 
                && activeItem.HasScriptOfType<CardItem>())
            {
                activeItem.transform.position = transform.position;
            }
        }
    }
}
