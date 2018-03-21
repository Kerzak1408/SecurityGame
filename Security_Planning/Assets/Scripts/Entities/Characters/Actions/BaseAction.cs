using Assets.Scripts.Entities.Characters;

namespace Entities.Characters.Actions
{
    public abstract class BaseAction
    {
        protected BaseCharacter character;

        protected BaseAction(BaseCharacter character)
        {
            this.character = character;
        }

        public bool IsCompleted { get; protected set; }
        public abstract void Activate();
        public abstract void Update();
    }
}

