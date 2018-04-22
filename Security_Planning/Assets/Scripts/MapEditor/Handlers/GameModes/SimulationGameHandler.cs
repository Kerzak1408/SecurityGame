using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters.Actions;
using Assets.Scripts.Entities.Characters.Behaviours;
using Assets.Scripts.Entities.Characters.Goals;
using Assets.Scripts.MapEditor;
using Assets.Scripts.Model;

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
        PlanningNode[] startNodes = new PlanningNode[actionsToDraw.Length];
        while (goals.Count > 0)
        {
            
            NavigationGoal goal = goals.Dequeue() as NavigationGoal;

            for (int i = 0; i < startNodes.Length; i++)
            //for (int i = 0; i < 1; i++)
            {
                goal.Reset();
                PlanningNode startNode = startNodes[i];
                if (startNode != null)
                {
                    startNode.Reset();
                }
                Path<PlanningNode, PlanningEdge> currentPath;

                goal.MaxVisibility = (float)i / (startNodes.Length - 1);
                //goal.MaxVisibility = 1.0f;
                goal.Activate(startNode);

                while (!goal.IsInitialized)
                {
                    yield return null;
                }
                currentPath = goal.Path;
                if (currentPath.GoalNode != null)
                {
                    startNodes[i] = currentPath.GoalNode.Copy();
                }
                
                if (currentPath.Edges == null)
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
