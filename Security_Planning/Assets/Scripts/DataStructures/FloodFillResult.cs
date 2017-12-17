using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    public class FloodFillResult
    {
        public List<IntegerTuple> Coordinates;
        /// <summary>
        /// Is this flood fill area bounded?
        /// </summary>
        public bool IsRoom;

        public FloodFillResult()
        {
            IsRoom = true;
            Coordinates = new List<IntegerTuple>();
        }
    }
}
