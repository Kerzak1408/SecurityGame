using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Model;

namespace Entities.Characters.Goals
{
    public abstract class BaseGoal
    {
        public BaseCharacter Character { get; private set; }
        public IntegerTuple GoalCoordinates { get; private set; }
        public bool IsFinished { get; protected set; }

        protected BaseGoal(BaseCharacter character, IntegerTuple goalCoordinates)
        {
            Character = character;
            GoalCoordinates = goalCoordinates;
        }

        public abstract void Activate();
        public abstract void Update();
    }
}
