using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;

namespace Assets.Scripts.Model
{
    public class TileEdge : Edge<TileNode, EdgeType>
    {
        public IObstacle Obstacle { get; private set; }

        public bool IsOpen
        {
            get { return Obstacle == null || Obstacle.IsOpen; }
        }

        public TileEdge(TileNode start, TileNode neighbor, EdgeType type, float cost, IObstacle obstacle) : base(start, neighbor, type, cost)
        {
            Obstacle = obstacle;
        }

        public void Open(BaseCharacter character)
        {
            Obstacle.Open(character);
        }
    }
}
