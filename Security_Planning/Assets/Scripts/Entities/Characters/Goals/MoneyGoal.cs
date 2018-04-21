using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Characters.Goals;
using Assets.Scripts.Model;
using UnityEngine;

public class MoneyGoal : NavigationGoal
{

    private GameObject moneyObject;

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
