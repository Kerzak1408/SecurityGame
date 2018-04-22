using System.Collections.Generic;
using Assets.Scripts.DataStructures;

namespace Assets.Scripts.Algorithms.AStar.Interfaces
{
    public interface IAStarNode<T> where T : IAStarNode<T>
    {
        IntegerTuple Position { get; set; }
        List<IAStarEdge<T>> Edges { get; }
        float TotalTime { get; set; }
    }
}
