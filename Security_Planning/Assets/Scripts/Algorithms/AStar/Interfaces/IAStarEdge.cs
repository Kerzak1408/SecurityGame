namespace Assets.Scripts.Algorithms.AStar.Interfaces
{
    public interface IAStarEdge<out TNode> where TNode : IAStarNode<TNode>
    {
        TNode Start { get; }
        TNode Neighbor { get; }
        float Cost { get; }
    }
}
