using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Model;
using Entities.Characters.Actions;
using Entities.Characters.Goals;
using UnityEngine;

public class MoneyGoal : BaseGoal
{
    private BaseAction currentAction;
    private Queue<BaseAction> actions;
    private GameObject moneyObject;

    public MoneyGoal(BaseCharacter character, IntegerTuple goalCoordinates, GameObject moneyObject) : base(character, goalCoordinates, PlanningEdgeType.MONEY)
    {
        this.moneyObject = moneyObject;
    }

    public override void Activate()
    {
        Map currentMap = Character.CurrentGame.Map;
        PlanningNode startNode, goalNode;
        currentMap.GetPlanningModel(Character, GoalCoordinates, moneyObject, out startNode, out goalNode);
        Path<PlanningNode, PlanningEdge> plannedPath = AStarAlgorithm.AStar<PlanningNode, PlanningEdge>(
            startNode,
            goalNode,
            new EuclideanHeuristics<PlanningNode>(currentMap.Tiles),
            Debug.Log,
            edgeFilter: edge => Character.Data.ForbiddenPlanningEdgeTypes.Contains(edge.Type));
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
    }

    public override void Update()
    {
        if (IsFinished) return;
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
