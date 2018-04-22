namespace Assets.Scripts.Entities.Characters.Behaviours
{
    public abstract class BaseBehaviour
    {
        protected BaseCharacter Character;
        public bool IsInitialized { get; protected set; }

        protected BaseBehaviour(BaseCharacter character)
        {
            Character = character;
        }

        public abstract void Start();

        public abstract void Update();
    }
}

