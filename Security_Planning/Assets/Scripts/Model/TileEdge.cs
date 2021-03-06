﻿using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters;

namespace Assets.Scripts.Model
{
    /// <summary>
    /// Navigation edge.
    /// </summary>
    public class TileEdge : Edge<TileNode, EdgeType>
    {
        private readonly float cost;

        public IObstacle Obstacle { get; private set; }
        public List<BaseEntity> ObstructingEntities { get; set; }

        public override float Cost
        {
            get { return cost; }
        }

        public bool IsOpen
        {
            get { return Obstacle == null || Obstacle.IsOpen; }
        }

        public TileEdge(TileNode start, TileNode neighbor, EdgeType type, float cost, IObstacle obstacle) : base(start, neighbor, type)
        {
            Obstacle = obstacle;
            this.cost = cost;
            ObstructingEntities = new List<BaseEntity>();
        }

        public void Open(BaseCharacter character)
        {
            Obstacle.Open(character);
        }

        public bool IsObstructed(IEnumerable<BaseEntity> destroyedObstacles)
        {
            return ObstructingEntities.Any(obstacle => !destroyedObstacles.Contains(obstacle));
        }
    }
}
