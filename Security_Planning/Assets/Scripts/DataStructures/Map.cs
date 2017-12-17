using System;
using System.Collections.Generic;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    public class Map
    {
        public static readonly string UP = "Up";
        public static readonly string DOWN = "Down";
        public static readonly string LEFT = "Left";
        public static readonly string RIGHT = "Right";
        public static readonly string ROLLING_DOOR = "RollingDoor";
        public static readonly string KEY_GATE = "KeyGate";
        public static readonly string WINDOW = "Window";
        public static readonly string FENCE = "Fence";
        public static readonly string GATE = "Gate";
        public static readonly string HEDGE = "Hedge";

        public GameObject[,] Tiles { get; private set; }
        public List<GameObject> Entities { get; private set; }
        public Dictionary<Tuple<int, int>, string> PasswordDictionary { get; private set; }
        public GameObject EmptyParent { get; private set; }
        public AIModel AIModel { get; private set; }

        public Map(GameObject[,] tiles, List<GameObject> entities, GameObject emptyParent, Dictionary<Tuple<int, int>, string> passwordDictionary)
        {
            Tiles = tiles;
            Entities = entities;
            EmptyParent = emptyParent;
            PasswordDictionary = passwordDictionary;
            AIModel = ExtractAIModel();
        }

        public void SetActive(bool active)
        {
            EmptyParent.SetActive(active);
        }

        public void RemoveEntity(GameObject toBeRemovedEntity)
        {
            Entities.Remove(toBeRemovedEntity);
        }

        /// <summary>
        /// Set active all entities within the map that has a script of type <typeparamref name="T"/>.
        /// Deactivate all the other entities.
        /// </summary>
        /// <typeparam name="T"> Type of the scripts defining the entities that will be activated. At least <see cref="MonoBehaviour"/> </typeparam>
        public void DeactivateEntitiesExceptOfType(Type type) 
        {
            foreach (GameObject entity in Entities)
            {
                bool activate = entity.HasScriptOfType(type);
                entity.SetActive(activate);
            }
        }

        public void ActivateAllEntities()
        {
            foreach (GameObject entity in Entities)
            {
                entity.SetActive(true);
            }
        }

        public int GetNextEntityId()
        {
            var ids = new List<int>();
            foreach (GameObject entity in Entities)
            {
                string[] splitName = entity.name.Split('_');
                int id = int.Parse(splitName[splitName.Length - 1]);
                ids.Add(id);
            }
            for (int i = 0;; i++)
            {
                if (!ids.Contains(i))
                {
                    return i;
                }
            }
        }

        public AIModel ExtractAIModel()
        {
            int width = Tiles.GetLength(0);
            int height = Tiles.GetLength(1);
            AIModel result = new AIModel(width, height);
        
            // Horizontal and vertical edges
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    TileModel tileModel = new TileModel(i, j);
                    Tuple<int, int>[] neighbors =
                    {
                        Tuple.New(i - 1, j),
                        Tuple.New(i + 1, j),
                        Tuple.New(i, j - 1),
                        Tuple.New(i, j + 1)
                    };
                    foreach (Tuple<int, int> neighbor in neighbors)
                    {
                        Edge edge;
                        if ((edge = CanGetFromTo(i, j, neighbor.First, neighbor.Second)) != null)
                        {
                            // TODO: cost
                            tileModel.AddNeighbor(edge);
                        }
                    }
                    result.Tiles[i, j] = tileModel;
                }

            // Diagonal edges
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    IntegerTuple transition1 = new IntegerTuple(1, 0);
                    IntegerTuple transition2 = new IntegerTuple(0, 1);
                    TileModel tileModel = result.Tiles[i, j];
                    IntegerTuple tileModelPosition = tileModel.Position;
                    for (int first = -1; first <= 1; first += 2)
                        for (int second = -1; second <= 1; second += 2)
                        {
                            IntegerTuple currentTransition1 = transition1 * first;
                            IntegerTuple currentTransition2 = transition2 * second;
                            // We have to check 4 things: whether we can perform the first transition and the second afterwards
                            // and vice versa. E.g. if we check whether we can get to the left-up neighbor we need to be able to
                            // get to the left neighbor and from it to its up neighbor. Additionaly we need to be able to get to
                            // the up neighbor and from it to its left neighbor.
                            if (tileModel.HasDirectTransitionTo(tileModelPosition + currentTransition1) &&
                                result.Tiles.Get(tileModelPosition + currentTransition1)
                                    .HasDirectTransitionTo(tileModelPosition + currentTransition1 +
                                                           currentTransition2) &&
                                tileModel.HasDirectTransitionTo(tileModelPosition + currentTransition2) &&
                                result.Tiles.Get(tileModelPosition + currentTransition2)
                                    .HasDirectTransitionTo(tileModelPosition + currentTransition2 + currentTransition1))
                            {
                                tileModel.AddNeighbor(tileModelPosition + currentTransition2 + currentTransition1, EdgeType.NORMAL, Mathf.Sqrt(2));
                            }
                                
                        }
                }
            return result;
        }

        private Edge CanGetFromTo(int fromX, int fromY, int toX, int toY)
        {
            if (toX < 0 || toY < 0 || toX >= Tiles.GetLength(0) || toY >= Tiles.GetLength(1))
            {
                return null;
            }
            string fromName = Tiles[fromX, fromY].name;
            string toName = Tiles[toX, toY].name;
            string fromObstacleDirection = "";
            string toObstacleDirection = "";
            
            // Unreachable tiles.
            if (fromName.Contains(HEDGE) || toName.Contains(HEDGE))
            {
                return null;
            }
            if (toX == fromX)
            {
                // UP or DOWN
                if (toY > fromY)
                {
                    // UP
                    fromObstacleDirection = RIGHT;
                    toObstacleDirection = LEFT;
                    //anyObstacle = fromName.Contains(RIGHT) || toName.Contains(LEFT);
                }
                else
                {
                    // DOWN
                    fromObstacleDirection = LEFT;
                    toObstacleDirection = RIGHT;
                    //anyObstacle = fromName.Contains(LEFT) || toName.Contains(RIGHT);
                }
            }
            else if (toY == fromY)
            {
                // LEFT or RIGHT
                if (toX > fromX)
                {
                    // RIGHT
                    fromObstacleDirection = UP;
                    toObstacleDirection = DOWN;
                    //anyObstacle = fromName.Contains(UP) || toName.Contains(DOWN);
                }
                else
                {
                    // LEFT
                    fromObstacleDirection = DOWN;
                    toObstacleDirection = UP;
                    //anyObstacle = fromName.Contains(DOWN) || toName.Contains(UP);
                }
            }

            bool fromObstacle = fromName.Contains(fromObstacleDirection);
            bool toObstacle = toName.Contains(toObstacleDirection);



            // Is there an obstacle between tiles?
            if (fromObstacle || toObstacle)
            {
                EdgeType fromType = EdgeTypeUtils.ParseString(fromName);
                EdgeType toType = EdgeTypeUtils.ParseString(toName);
                EdgeType edgeType = EdgeType.NONE;
                if (fromObstacle && !fromType.Equals(EdgeType.NONE)) edgeType = fromType;
                if (toObstacle && !toType.Equals(EdgeType.NONE)) edgeType = toType;
                return edgeType.Equals(EdgeType.NONE) ? null : new Edge(toX, toY, edgeType, 1);
            }
            else
            {
                return new Edge(toX, toY, EdgeType.NORMAL, 1);
            }
        }
    } 
}
