using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;

public interface IPlanningEdgeCreator
{
    PlanningEdgeType PlanningEdgeType { get; }

    GameObject GameObject { get; }

    /// <summary>
    /// Used in planning graph building. Tells whether the edge of type corresponding to
    /// this instance leading from <paramref name="node"/> should be added.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    bool ShouldExplore(PlanningNode node);

    void ModifyNextNode(PlanningNode node);
}
