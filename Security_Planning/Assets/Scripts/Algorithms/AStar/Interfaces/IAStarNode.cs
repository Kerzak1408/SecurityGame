using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAStarNode
{
    List<IAStarEdge> Edges { get; }
}
