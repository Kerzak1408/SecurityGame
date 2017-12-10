using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    public class FloodFillResult
    {
        public List<Vector2> Coordinates;
        public bool IsRoom;

        public FloodFillResult()
        {
            IsRoom = true;
            Coordinates = new List<Vector2>();
        }
    }
}
