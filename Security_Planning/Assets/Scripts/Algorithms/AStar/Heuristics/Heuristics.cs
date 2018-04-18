using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;

public abstract class Heuristics<T> where T : IAStarNode<T>
{
    public abstract PriorityCost ComputeHeuristics(T from, T to, int priorityCostLength);
}
