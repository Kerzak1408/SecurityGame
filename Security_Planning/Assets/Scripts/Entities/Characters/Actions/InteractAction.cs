using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Entities.Characters;
using UnityEngine;

namespace Entities.Characters.Actions
{
    public class InteractAction : BaseAction
    {

        public GameObject Interacted { get; private set; }

        public string InteractedName { get;  set; }

        public InteractAction(BaseCharacter character, GameObject interacted) : base(character)
        {
            this.Interacted = interacted;
        }

        public override void Activate()
        {
            character.Log("Trying to interact with " + Interacted.name + ".");
            character.InteractWith(Interacted, () =>
            {
                character.Log("Successfully interacted with " + Interacted.name + ".");
                IsCompleted = true;
            });
        }

        public override void Update()
        {
        }
    }
}
