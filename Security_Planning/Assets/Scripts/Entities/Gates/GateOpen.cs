using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Items;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
    public class GateOpen : BaseSlidingGate, IObstacle
    {
        public CardReaderEntity RelatedCardReader { get; set; }

        public EdgeType EdgeType
        {
            get { return RelatedCardReader == null ? EdgeType.DOOR : EdgeType.CARD_DOOR; }
        }

        public IInteractable InteractableObject
        {
            get { return RelatedCardReader; }
        }

        public bool IsOpen
        {
            get { return IsGateFullyOpen; }
        }

        public float DelayTime
        {
            get { return slidingDoorOpen.clip.length; }
        }

        public void Open(BaseCharacter character)
        {
            if (RelatedCardReader != null)
            {
                character.ActivateItem<CardItem>();
                RelatedCardReader.Interact(character);
            }
        }
    }
}
