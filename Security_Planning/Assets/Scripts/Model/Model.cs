using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities;

namespace Assets.Scripts.Model
{
    public class AIModel
    {
        public TileNode[,] Tiles { get; set; }

        public AIModel(int width, int height)
        {
            Tiles = new TileNode[width, height];
        }

        public void Reset()
        {
            foreach (TileNode tileNode in Tiles)
            {
                tileNode.Reset();
            }
        }
    }
}
