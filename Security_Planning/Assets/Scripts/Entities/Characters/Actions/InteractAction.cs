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
            character.InteractWith(interacted, () => IsCompleted = true);
        }

        public override void Update()
        {
        }
    }
}
