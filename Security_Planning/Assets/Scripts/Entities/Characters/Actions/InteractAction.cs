using UnityEngine;

namespace Assets.Scripts.Entities.Characters.Actions
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
            Character.Log("Trying to interact with " + Interacted.name + ".");
            Character.InteractWith(Interacted, () =>
            {
                Character.Log("Successfully interacted with " + Interacted.name + ".");
                IsCompleted = true;
            });
        }

        public override void Update()
        {
        }
    }
}
