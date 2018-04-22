using System;
using System.Collections.Generic;
using Assets.Scripts.Model;

namespace Assets.Scripts.Serialization
{
    [Serializable]
    public class CharacterData : BaseEntityData
    {
        private string[] itemNames;
        private List<EdgeType> forbiddenEdgeTypes;
        private List<PlanningEdgeType> forbiddenPlanningEdgeTypes;
        [Obsolete("Maintained only because of backward compatibility.")]
        private List<DetectorType> ignoredDetectors;

        private float maxVisibilityMeasure;
        private int sensitivity;

        public string[] ItemNames
        {
            get { return itemNames ?? (itemNames = new string[0]); }
            set { itemNames = value; }
        }

        public List<EdgeType> ForbiddenEdgeTypes
        {
            get { return forbiddenEdgeTypes ?? (forbiddenEdgeTypes = new List<EdgeType>()); }
            set { forbiddenEdgeTypes = value; }
        }

        public List<PlanningEdgeType> ForbiddenPlanningEdgeTypes
        {
            get { return forbiddenPlanningEdgeTypes ?? (forbiddenPlanningEdgeTypes = new List<PlanningEdgeType>()); }
            set { forbiddenPlanningEdgeTypes = value; }
        }

        public float MaxVisibilityMeasure
        {
            get { return maxVisibilityMeasure; }
            set { maxVisibilityMeasure = value; }
        }

        public int Sensitivity
        {
            get { return sensitivity; }
            set { sensitivity = value; }
        }
    }
}
