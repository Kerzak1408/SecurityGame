using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAStarEdge
{
    IAStarNode Neighbor { get; }
    float Cost { get; }
}
