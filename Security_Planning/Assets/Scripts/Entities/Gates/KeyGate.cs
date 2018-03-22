using System;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Entities.Gates
{
    public class KeyGate : BaseSlidingGate, IInteractable, IObstacle
    {

        public EdgeType EdgeType
        {
            get { return EdgeType.KEY_DOOR; }
        }
        public IInteractable InteractableObject
        {
            get { return this; }
        }

        public bool IsOpen
        {
            get { return IsGateFullyOpen; }
        }

        protected override void Start()
        {
            base.Start();
            Lock();
        }

        public void Interact(BaseCharacter character, Action successAction = null)
        {
            GameObject activeItemObject = character.GetActiveItem();
            if (activeItemObject.HasScriptOfType<KeyItem>())
            {
                UnlockOnce();
            }
        }

        public void Open(BaseCharacter character)
        {
            character.ActivateItem<KeyItem>();
            Interact(character);
        }

    }
}
