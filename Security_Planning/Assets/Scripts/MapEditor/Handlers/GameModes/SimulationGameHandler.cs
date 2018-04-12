using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters.Goals;
using Assets.Scripts.MapEditor;
using Assets.Scripts.Model;
using Entities.Characters.Actions;
using Entities.Characters.Behaviours;
using Entities.Characters.Goals;
using UnityEngine;

public class SimulationGameHandler : BaseGameHandler
{
    private bool isStarted;

    public override string Name
    {
        get { return "Simulation"; }
    }

    public override void Start(Game game)
    {
        base.Start(game);
        Game.PanelSimulation.SetActive(true);
    }

    public override void Update()
    {
        if (isStarted)
        {
            return;
        }
        Game.StartCoroutine(WaitForGoalPlanning());
        isStarted = true;
    }

    private IEnumerator WaitForGoalPlanning()
    {
        var actionsToDraw = new List<BaseAction>[NavigationGoal.PATHS_COUNT];
        for (int i = 0; i < actionsToDraw.Length; i++)
        {
            actionsToDraw[i] = new List<BaseAction>();
        }
        
        Queue<BaseGoal> goals = CollectEverythingBehaviour.GenerateGoals(Burglar, false);
        NavigationGoal previousGoal = null;

        while (goals.Count > 0)
        {
            Path<PlanningNode, PlanningEdge>[] nullPaths = null;
            NavigationGoal goal = goals.Dequeue() as NavigationGoal;
            PlanningNode[] startNodes = new PlanningNode[actionsToDraw.Length];
            if (previousGoal != null)
            {
                for (int i = 0; i < startNodes.Length; i++)
                {
                    startNodes[i] = previousGoal.PossiblePaths[i].GoalNode;
                }
            }
            previousGoal = goal;

            for (int i = 0; i < startNodes.Length; i++)
            {
                PlanningNode startNode = startNodes[i];
                Path<PlanningNode, PlanningEdge> currentPath;
                if (startNode != null || nullPaths == null)
                {
                    goal.Activate(startNode);

                    while (!goal.IsInitialized)
                    {
                        yield return null;
                    }
                    currentPath = goal.PossiblePaths[i];
                    if (startNode == null)
                    {
                        nullPaths = goal.PossiblePaths;
                    }
                }
                else
                {
                    currentPath = nullPaths[i];
                }
                
                if (currentPath == null || currentPath.Edges == null)
                {
                    continue;
                }

                foreach (PlanningEdge planningEdge in currentPath.Edges)
                {
                    foreach (BaseAction action in planningEdge.ActionsToComplete)
                    {
                        if (action.GetType() == typeof(InteractAction))
                        {
                            var interactAction = (InteractAction)action;
                            interactAction.InteractedName = interactAction.Interacted.name;
                        }
                        actionsToDraw[i].Add(action);
                    }
                }
            }
        }
        Game.EndSimulation(actionsToDraw);
    }
}
