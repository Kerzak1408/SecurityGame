using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Items;
using UnityEngine;

public interface IAStarEdge<TNode> where TNode : IAStarNode<TNode>
{
    TNode Neighbor { get; }
    float Cost { get; }
}
