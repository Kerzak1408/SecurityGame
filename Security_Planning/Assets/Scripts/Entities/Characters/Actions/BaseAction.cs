namespace Assets.Scripts.Entities.Characters.Actions
{
    public abstract class BaseAction
    {
        protected BaseCharacter Character;

        protected BaseAction(BaseCharacter character)
        {
            Character = character;
        }

        public bool IsCompleted { get; protected set; }
        public abstract void Activate();
        public abstract void Update();
    }
}

