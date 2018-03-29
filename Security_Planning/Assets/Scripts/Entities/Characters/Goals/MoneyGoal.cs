using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Model;
using Entities.Characters.Actions;
using Entities.Characters.Goals;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MoneyGoal : BaseGoal
{
    private BaseAction currentAction;
    private Queue<BaseAction> actions;
    private GameObject moneyObject;
    private bool isInitialized;

    public MoneyGoal(BaseCharacter character, IntegerTuple goalCoordinates, GameObject moneyObject) : base(character, goalCoordinates, PlanningEdgeType.MONEY)
    {
        this.moneyObject = moneyObject;
    }

    public override void Activate()
    {
        Map currentMap = Character.CurrentGame.Map;
        PlanningNode startNode, goalNode;
        currentMap.GetPlanningModel(Character, GoalCoordinates, moneyObject, out startNode, out goalNode);
        Debug.Log("Starting new planning thread.");
        Thread planningThread = new Thread(() => PlanPath(startNode, goalNode, currentMap));
        planningThread.Start();
    }

    private void PlanPath(PlanningNode startNode, PlanningNode goalNode, Map currentMap)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Path<PlanningNode, PlanningEdge> plannedPath = AStarAlgorithm.AStar<PlanningNode, PlanningEdge>(
            startNode,
            goalNode,
            new EuclideanHeuristics<PlanningNode>(currentMap.Tiles),
            edgeFilter: edge => Character.Data.ForbiddenPlanningEdgeTypes.Contains(edge.Type));
        stopwatch.Stop();
        Debug.Log("A* time = " + stopwatch.ElapsedMilliseconds / 1000f + "seconds.");
        TaskManager.Instance.RunOnMainThread(() => Initialize(plannedPath));
    }

    private void Initialize(Path<PlanningNode, PlanningEdge> plannedPath)
    {
        if (plannedPath.Cost == float.MaxValue || plannedPath.Edges == null)
        {
            IsFinished = true;
        }
        else
        {
            actions = new Queue<BaseAction>();
            foreach (PlanningEdge planningEdge in plannedPath.Edges)
            {
                List<BaseAction> edgeActions = planningEdge.ActionsToComplete;
                foreach (BaseAction edgeAction in edgeActions)
                {
                    actions.Enqueue(edgeAction);
                }
            }
        }

        isInitialized = true;
    }

    public override void Update()
    {
        if (!isInitialized || IsFinished) return;
        if (currentAction == null || currentAction.IsCompleted)
        {
            if (actions.Count > 0)
            {
                currentAction = actions.Dequeue();
                currentAction.Activate();
            }
            else
            {
                IsFinished = true;
            }
        }
        currentAction.Update();
    }
}
