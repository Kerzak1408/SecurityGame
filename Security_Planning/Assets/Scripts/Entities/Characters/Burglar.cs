using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class Burglar : BaseCharacter {

    float probability;
    Vector3 directionVector;
    CharacterController controller;
    private Queue<TileNode> path;
    private TileNode followedNode;
    private bool isPaused = true;
    private EuclideanHeuristics heuristics;
    TileNode[,] aiModelTiles;
    private Queue<IntegerTuple> goals;
    private IntegerTuple currentGoal;
    private bool waitingForNextNode = true;

    // Use this for initialization
    public override void StartGame()
    { 
	    heuristics = new EuclideanHeuristics(CurrentGame.Map.Tiles);
	    aiModelTiles = CurrentGame.Map.AIModel.Tiles;
        goals = new Queue<IntegerTuple>();
        goals.Enqueue(new IntegerTuple(5, 5));
        goals.Enqueue(new IntegerTuple(5, 0));
        //path = new Queue<TileNode>(AStarAlgorithm.AStar(aiModelTiles[0, 0], aiModelTiles[5, 5], heuristics, Debug.Log, node => node.IsDetectable()));
        //followedNode = path.Dequeue();
    }

	// Update is called once per frame
    protected override void Update()
    {
        base.Update();
        ProcessInputs();
        isMoving = false;
        if (isPaused) return;
        if (currentGoal == null)
        {
            if (goals.Count > 0) currentGoal = goals.Dequeue();
            else return;
        }
        if (followedNode != null)
        {
            if (NavigateTo(followedNode))
            {
                if (waitingForNextNode)
                {
                    RecomputePath();
                    followedNode = path.Count == 0 ? null : path.Dequeue();
                }
            }
            else
            {
                waitingForNextNode = true;
            }
        }
        else
        {
            RecomputePath();
        }
    }

    private void RecomputePath()
    {
        Map currentMap = CurrentGame.Map;
        currentMap.ExtractAIModel();
        aiModelTiles = CurrentGame.Map.AIModel.Tiles;
        TileNode startNode;
        if (followedNode == null)
        {
            startNode = currentMap.GetClosestTile(transform.position);
        }
        else
        {
            startNode = aiModelTiles[followedNode.Position.First, followedNode.Position.Second];
        }
        List<TileNode> fullPath = AStarAlgorithm.AStar(startNode, aiModelTiles[currentGoal.First, currentGoal.Second], heuristics, Debug.Log, node => node.IsDetectable());
        if (fullPath == null || fullPath.Count <= 1)
        {
            currentGoal = goals.Count == 0 ? null : goals.Dequeue();
            followedNode = null;
        }
        else
        {
            path = new Queue<TileNode>(fullPath);
            followedNode = path.Dequeue();
        }

    }

    private void ProcessInputs()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
        }
    }

    private bool NavigateTo(TileNode tileNode)
    {
        Vector3 target = CurrentGame.Map.Tiles.Get(tileNode.Position).transform.position;
        transform.LookAt(target);
        MoveForward();
        return (Vector3.Distance(transform.position, target) < 0.05f);
    }

    public override void RequestPassword(IPasswordOpenable passwordOpenableObject)
    {
        throw new System.NotImplementedException();
    }

    public override void InterruptRequestPassword()
    {
        throw new System.NotImplementedException();
    }

    public override void RequestCard(CardReaderEntity cardReader)
    {
        cardReader.VerifyCard();
    }
    
}
