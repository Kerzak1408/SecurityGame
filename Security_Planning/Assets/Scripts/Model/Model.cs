using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Model
{
    public class AIModel
    {
        public TileNode[,] Tiles { get; set; }

        public List<ClusterNode> ContractedNodes
        {
            get
            {
                return FloodFillAlgorithm.GenerateClusters<ClusterNode, TileNode>(Tiles.ToEnumerable<TileNode>(),
                    node => node.Edges.Where(edge => !edge.IsObstructed(new List<BaseEntity>())).Select(edge => edge.Neighbor)
                        .Where(neighbor => node.IsDetectable() == neighbor.IsDetectable()));
            }
        }

        public AIModel(int width, int height)
        {
            Tiles = new TileNode[width, height];
        }
    }
}
