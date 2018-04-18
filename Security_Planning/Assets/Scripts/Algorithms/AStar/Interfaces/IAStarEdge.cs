using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Items;
using UnityEngine;

public interface IAStarEdge<TNode> where TNode : IAStarNode<TNode>
{
    TNode Start { get; }
    TNode Neighbor { get; }
    float Cost { get; }
}
