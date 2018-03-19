using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Entities.Characters;
using UnityEngine;

namespace Entities.Characters.Goals
{
    public class InteractGoal : BaseGoal
    {
        private GameObject interacted;

        public InteractGoal(BaseCharacter character, GameObject interacted) : base(character)
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
