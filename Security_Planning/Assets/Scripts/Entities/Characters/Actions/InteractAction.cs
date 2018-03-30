using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Entities.Characters;
using UnityEngine;

namespace Entities.Characters.Actions
{
    public class InteractAction : BaseAction
    {
        private GameObject interacted;

        public InteractAction(BaseCharacter character, GameObject interacted) : base(character)
        {
            this.interacted = interacted;
        }

        public override void Activate()
        {
            character.Log("Trying to interact with " + interacted.name + ".");
            character.InteractWith(interacted, () =>
            {
                character.Log("Successfully interacted with " + interacted.name + ".");
                IsCompleted = true;
            });
        }

        public override void Update()
        {
        }
    }
}
