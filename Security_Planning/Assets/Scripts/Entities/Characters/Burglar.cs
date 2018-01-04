using System.Collections.Generic;
using System.Linq;
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

	// Use this for initialization
    public override void StartGame()
    { 
	    EuclideanHeuristics heuristics = new EuclideanHeuristics(CurrentGame.Map.Tiles);
	    TileNode[,] aiModelTiles = CurrentGame.Map.AIModel.Tiles;
	    path = new Queue<TileNode>(AStarAlgorithm.AStar(aiModelTiles[0, 0], aiModelTiles[5, 5], heuristics, Debug.Log));
	    followedNode = path.Dequeue();
	}

	// Update is called once per frame
    protected override void Update()
    {
        base.Update();
        isMoving = false;
        if (followedNode != null)
        {
            if (NavigateTo(followedNode))
            {
                followedNode = path.Count == 0 ? null : path.Dequeue();
            }
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
