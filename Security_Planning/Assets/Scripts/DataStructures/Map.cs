using System;
using System.Collections.Generic;
using Assets.Scripts.Extensions;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    public class Map
    {
        private static readonly string UP = "Up";
        private static readonly string DOWN = "Down";
        private static readonly string LEFT = "Left";
        private static readonly string RIGHT = "Right";
        private static readonly string ROLLING_DOOR = "RollingDoor";
        private static readonly string KEY_GATE = "KeyGate";
        private static readonly string WINDOW = "Window";
        private static readonly string FENCE = "Fence";
        private static readonly string GATE = "Gate";

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
            bool anyObstacle = false;
            if (toX == fromX)
            {
                // UP or DOWN
                if (toY > fromY)
                {
                    // UP
                    anyObstacle = fromName.Contains(RIGHT) || toName.Contains(LEFT);
                }
                else
                {
                    // DOWN
                    anyObstacle = fromName.Contains(LEFT) || toName.Contains(RIGHT);
                }
            }
            else if (toY == fromY)
            {
                // LEFT or RIGHT
                if (toX > fromX)
                {
                    // RIGHT

                    anyObstacle = fromName.Contains(UP) || toName.Contains(DOWN);
                }
                else
                {
                    // LEFT

                    anyObstacle = fromName.Contains(DOWN) || toName.Contains(UP);
                }
            }
            //else if (toX > fromX)
            //{
            //    // UP-RIGHT or DOWN-RIGHT
            //    if (toY > fromY)
            //    {
            //        // UP-RIGHT

            //    }
            //    else
            //    {
            //        // DOWN-RIGHT
            //    }
            //}
            //else
            //{
            //    // UP-LEFT or DOWN-LEFT
            //    if (toY > fromY)
            //    {
            //        // UP-LEFT
            //    }
            //    else
            //    {
            //        // DOWN-LEFT
            //    }
            //}

            if (anyObstacle)
            {
                if (fromName.Contains(WINDOW) || toName.Contains(WINDOW))
                {
                    return new Edge(toX, toY, EdgeType.WINDOW, 1);
                }
                if (fromName.Contains(FENCE) || toName.Contains(FENCE))
                {
                    return new Edge(toX, toY, EdgeType.FENCE, 1);
                }
                if (fromName.Contains(KEY_GATE) || toName.Contains(KEY_GATE) ||
                    fromName.Contains(ROLLING_DOOR) || toName.Contains(ROLLING_DOOR))
                {
                    return new Edge(toX, toY, EdgeType.KEY_DOOR, 1);
                }
                if (fromName.Contains(WINDOW) || toName.Contains(WINDOW))
                {
                    return new Edge(toX, toY, EdgeType.WINDOW, 1);
                }
                if (fromName.Contains(GATE) || toName.Contains(GATE))
                {
                    return new Edge(toX, toY, EdgeType.DOOR, 1);
                }
            }
            else
            {
                return new Edge(toX, toY, EdgeType.NORMAL, 1);
            }


            return null;
        }
    } 
}
