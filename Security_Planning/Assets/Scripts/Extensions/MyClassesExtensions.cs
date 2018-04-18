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
        public static float VisibleTime(this Path<PlanningNode, PlanningEdge> planningPath)
        {
            float measureSum = 0;
            if (planningPath.Edges == null)
            {
                return float.MaxValue;
            }
            return planningPath.Edges.Sum(edge => edge.VisibleTime);
        }
    }
}
