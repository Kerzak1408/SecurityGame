using System;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Gates;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using UnityEngine;
using UnityEngine.UI;

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
            if (other.gameObject.HasScriptOfType<CardItem>())
            {
                relatedGate.UnlockOnce();
                //BaseCharacter baseCharacter = other.gameObject.GetComponent<BaseCharacter>();
                //baseCharacter.RequestCard(this);
            }
        }

        public void Interact(BaseCharacter character)
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
