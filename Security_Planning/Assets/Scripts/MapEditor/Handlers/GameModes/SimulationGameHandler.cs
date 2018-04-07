using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.Entities.Characters.Goals;
using Assets.Scripts.MapEditor;
using Assets.Scripts.Model;
using Entities.Characters.Actions;
using Entities.Characters.Behaviours;
using Entities.Characters.Goals;
using UnityEngine;

public class SimulationGameHandler : BaseGameHandler
{
    public override string Name
    {
        get { return "Simulation"; }
    }

    public override void Start(Game game)
    {
        base.Start(game);
        Game.PanelSimulation.SetActive(true);
        Game.StartCoroutine(WaitForGoalPlanning());
    }

    private IEnumerator WaitForGoalPlanning()
    {
        var actionsToDraw = new List<BaseAction>();
        Queue<BaseGoal> goals = CollectEverythingBehaviour.GenerateGoals(Burglar, false);
        NavigationGoal previousGoal = null;
        while (goals.Count > 0)
        {
            NavigationGoal goal = goals.Dequeue() as NavigationGoal;
            PlanningNode startNode = null;
            if (previousGoal != null)
            {
                startNode = previousGoal.GoalNode;
            }
            goal.Activate(startNode);
            while (!goal.IsInitialized)
            {
                yield return null;
            }
            if (goal.Path == null || goal.Path.Edges == null)
            {
                continue;
            }
            previousGoal = goal;
            foreach (PlanningEdge planningEdge in goal.Path.Edges)
            {
                foreach (BaseAction action in planningEdge.ActionsToComplete)
                {
                    if (action.GetType() == typeof(InteractAction))
                    {
                        var interactAction = (InteractAction)action;
                        interactAction.InteractedName = interactAction.Interacted.name;
                    }
                    actionsToDraw.Add(action);
                }
            }
        }
        Game.EndSimulation(actionsToDraw);
    }
}
