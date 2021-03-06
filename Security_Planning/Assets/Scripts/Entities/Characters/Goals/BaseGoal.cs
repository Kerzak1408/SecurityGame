﻿using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;

namespace Assets.Scripts.Entities.Characters.Goals
{
    public abstract class BaseGoal
    {
        public BaseCharacter Character { get; private set; }
        public IntegerTuple GoalCoordinates { get; private set; }
        public bool IsFinished { get; protected set; }
        public bool IsSuccessFul { get; protected set; }

        protected BaseGoal(BaseCharacter character, IntegerTuple goalCoordinates)
        {
            Character = character;
            GoalCoordinates = goalCoordinates;
        }

        public abstract void Activate(PlanningNode startNode=null);
        public abstract void Update();
    }
}
