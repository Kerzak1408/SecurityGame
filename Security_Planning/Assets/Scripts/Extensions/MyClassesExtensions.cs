using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Model;

namespace Assets.Scripts.Extensions
{
    public static class MyClassesExtensions
    {
        public static float VisibilityMeasure(this Path<PlanningNode, PlanningEdge> planningPath)
        {
            float measureSum = 0;
            float weightSum = 0;
            if (planningPath.Edges == null)
            {
                return float.MaxValue;
            }
            foreach (PlanningEdge planningEdge in planningPath.Edges)
            {
                measureSum += planningEdge.PathLength * planningEdge.VisibilityMeasure;
                weightSum += planningEdge.PathLength;
            }
            return measureSum / weightSum;
        }
    }
}
