using Assets.Scripts.Entities.Characters;

namespace Entities.Characters.Goals
{
    public abstract class BaseGoal
    {
        protected BaseCharacter character;

        protected BaseGoal(BaseCharacter character)
        {
            this.character = character;
        }

        public bool IsCompleted { get; protected set; }
        public abstract void Activate();
        public abstract void Update();
    }
}

