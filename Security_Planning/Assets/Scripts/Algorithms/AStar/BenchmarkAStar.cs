using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BenchmarkAStar
{
    public static int PlanningNodesExploited { get; set; }
    public static int PlanningEdgesAccesed { get; set; }
    public static int NavigationAstars { get; set; }
    public static int NavigationAstarsMultiple { get; set; }
    public static int NavigationNodesExploited { get; set; }
    public static int NavigationEdgesAccessed { get; set; }

    public static void Reset()
    {
        NavigationAstarsMultiple = 0;
        PlanningNodesExploited = 0;
        PlanningEdgesAccesed = 0;
        NavigationAstars = 0;
        NavigationNodesExploited = 0;
        NavigationEdgesAccessed = 0;
    }

    public static string StringResult()
    {
        return string.Join(" ", new string[] {
            "PlanningNodes =",PlanningNodesExploited.ToString(),
            "PlanningEdges =", PlanningEdgesAccesed.ToString(),
            "NavigationAstarsMultiple =", NavigationAstarsMultiple.ToString(),
            "NavigationAStars =", NavigationAstars.ToString(),
            "NavigationNodes =", NavigationNodesExploited.ToString(),
            "NavigationEdges =", NavigationEdgesAccessed.ToString()
        });
    }
}
