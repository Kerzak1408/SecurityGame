using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Characters.Goals;
using Assets.Scripts.Model;
using Entities.Characters.Actions;
using Entities.Characters.Goals;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MoneyGoal : NavigationGoal
{

    private GameObject moneyObject;

    public MoneyGoal(BaseCharacter character, IntegerTuple goalCoordinates, GameObject moneyObject) : 
        base(character, goalCoordinates)
    {
        this.moneyObject = moneyObject;
    }

    public override void Activate()
    {
        Character.Log("Goal activated: Collect money at " + GoalCoordinates);
        StartPlanning(moneyObject);
    }

    
}
