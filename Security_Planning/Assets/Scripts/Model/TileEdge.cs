using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Entities.Interfaces;

namespace Assets.Scripts.Model
{
    public class TileEdge : Edge<TileNode, EdgeType>
    {
        public IInteractable Interactable { get; private set; }

        public TileEdge(TileNode start, TileNode neighbor, EdgeType type, float cost, IInteractable interactable) : base(start, neighbor, type, cost)
        {
            Interactable = interactable;
        }
    }
}
