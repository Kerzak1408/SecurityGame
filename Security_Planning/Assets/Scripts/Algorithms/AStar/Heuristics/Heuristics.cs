using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Heuristics<T> where T : IAStarNode<T>
{
    public abstract float ComputeHeuristics(T from, T to);
}
