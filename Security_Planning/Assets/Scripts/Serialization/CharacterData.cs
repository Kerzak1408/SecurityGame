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
        private List<DetectorType> ignoredDetectors;

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

        public List<DetectorType> IgnoredDetectors
        {
            get { return ignoredDetectors ?? (ignoredDetectors = new List<DetectorType>()); }
            set { ignoredDetectors = value; }
        }
    }
}
