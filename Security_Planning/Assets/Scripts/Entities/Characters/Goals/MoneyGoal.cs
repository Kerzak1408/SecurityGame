using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Entities.Characters.Goals
{
    /// <summary>
    /// Navigate to the goal coordinates and interact with the moneyObject there.
    /// </summary>
    public class MoneyGoal : NavigationGoal
    {
        private readonly GameObject moneyObject;

        public MoneyGoal(BaseCharacter character, IntegerTuple goalCoordinates, GameObject moneyObject) : 
            base(character, goalCoordinates)
        {
            this.moneyObject = moneyObject;
        }

        public override void Activate(PlanningNode startNode=null)
        {
            Character.Log("Goal activated: Collect money at " + GoalCoordinates);
            StartPlanning(moneyObject, startNode);
        }
    }
}
