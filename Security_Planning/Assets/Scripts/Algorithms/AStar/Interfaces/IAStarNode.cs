using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAStarNode<T> where T : IAStarNode<T>
{
    List<IAStarEdge<T>> Edges { get; }
}
