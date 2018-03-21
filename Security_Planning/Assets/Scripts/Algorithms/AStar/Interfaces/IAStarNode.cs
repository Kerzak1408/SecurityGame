using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;

public interface IAStarNode<T> where T : IAStarNode<T>
{
    IntegerTuple Position { get; set; }
    List<IAStarEdge<T>> Edges { get; }
}
