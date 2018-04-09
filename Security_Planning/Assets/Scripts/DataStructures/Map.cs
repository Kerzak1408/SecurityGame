using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using Assets.Scripts.Items;
using Assets.Scripts.Model;
using Assets.Scripts.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

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
        private static readonly float MIN_OBSTACLE_DISTANCE = 0.5f;

        public GameObject[,] Tiles { get; private set; }
        public List<GameObject> Entities { get; private set; }
        public Dictionary<Tuple<int, int>, string> PasswordDictionary { get; private set; }
        public GameObject EmptyParent { get; private set; }
        public AIModel AIModel { get; private set; }
        public string Name { get; private set; }

        public Vector3 CenterWorld
        {
            get
            {
                Vector3 bottomLeft = Tiles[0, 0].transform.position;
                Vector3 upperRight = Tiles[Tiles.GetLength(0) - 1, Tiles.GetLength(1) - 1].transform.position;
                return (bottomLeft + upperRight) / 2;
            }
        }

        public int Width
        {
            get { return Tiles.GetLength(1); }
        }

        public int Height
        {
            get { return Tiles.GetLength(0); }
        }

        private Burglar burglar;
        public Burglar Burglar
        {
            get
            {
                return burglar ?? (burglar = Entities.First(entity => entity.HasScriptOfType<Burglar>())
                           .GetComponent<Burglar>());
            }
        }

        private Guard guard;
        public Guard Guard
        {
            get
            {
                return guard ?? (guard = Entities.First(entity => entity.HasScriptOfType<Guard>())
                           .GetComponent<Guard>());
            }
        }

        public Map(GameObject[,] tiles, List<GameObject> entities, GameObject emptyParent,
            Dictionary<Tuple<int, int>, string> passwordDictionary, string name)
        {
            Tiles = tiles;
            Entities = entities;
            EmptyParent = emptyParent;
            PasswordDictionary = passwordDictionary;
            Name = name;
        }

        public void SetActive(bool active)
        {
            EmptyParent.SetActive(active);
        }

        public void RemoveDetectorEntity(GameObject toBeRemovedEntity)
        {
            Entities.Remove(toBeRemovedEntity);
            foreach (TileNode node in AIModel.Tiles)
            {
                node.DetectedBy.Remove(toBeRemovedEntity.GetComponent<DetectorEntity>());
                foreach (TileEdge edge in node.Edges)
                {
                    edge.ObstructingEntities.Remove(toBeRemovedEntity.GetComponent<BaseEntity>());
                }
            }
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

        public void ExtractAIModel()
        {
            int width = Tiles.GetLength(0);
            int height = Tiles.GetLength(1);
            AIModel = new AIModel(width, height);

            // Nodes
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                AIModel.Tiles[i, j] = new TileNode(i, j, Tiles[i,j].transform.position);
            }

            // Horizontal and vertical edges
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                TileNode tileNode = AIModel.Tiles[i, j];
                Tuple<int, int>[] neighbors =
                {
                    Tuple.New(i - 1, j),
                    Tuple.New(i + 1, j),
                    Tuple.New(i, j - 1),
                    Tuple.New(i, j + 1)
                };
                foreach (Tuple<int, int> neighbor in neighbors)
                {
                    TileEdge edge;
                    if ((edge = CanGetFromTo(tileNode, i, j, neighbor.First, neighbor.Second)) != null)
                    {
                        // TODO: cost
                        tileNode.AddNeighbor(edge);
                    }
                }

            }

            // Diagonal edges
            for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                IntegerTuple transition1 = new IntegerTuple(1, 0);
                IntegerTuple transition2 = new IntegerTuple(0, 1);
                TileNode tileNode = AIModel.Tiles[i, j];
                IntegerTuple tileModelPosition = tileNode.Position;
                for (int first = -1; first <= 1; first += 2)
                for (int second = -1; second <= 1; second += 2)
                {
                    IntegerTuple currentTransition1 = transition1 * first;
                    IntegerTuple currentTransition2 = transition2 * second;
                    // We have to check 4 things: whether we can perform the first transition and the second afterwards
                    // and vice versa. E.g. if we check whether we can get to the left-up neighbor we need to be able to
                    // get to the left neighbor and from it to its up neighbor. Additionaly we need to be able to get to
                    // the up neighbor and from it to its left neighbor.
                    if (tileNode.HasDirectTransitionTo(tileModelPosition + currentTransition1) &&
                        AIModel.Tiles.Get(tileModelPosition + currentTransition1)
                            .HasDirectTransitionTo(tileModelPosition + currentTransition1 +
                                                   currentTransition2) &&
                        tileNode.HasDirectTransitionTo(tileModelPosition + currentTransition2) &&
                        AIModel.Tiles.Get(tileModelPosition + currentTransition2)
                            .HasDirectTransitionTo(tileModelPosition + currentTransition2 + currentTransition1))
                    {
                        IntegerTuple indices = tileModelPosition + currentTransition2 + currentTransition1;
                        TileNode neighbor = AIModel.Tiles[indices.First, indices.Second];
                        tileNode.AddNeighbor(neighbor, EdgeType.NORMAL, Mathf.Sqrt(2));
                    }

                }
            }

            // Remove edges that cannot be used because of obstacles.
            foreach (GameObject entity in Entities)
            {
                if (entity.HasScriptOfType<ItemEntity>()) continue;
                //Debug.Log("Potential obstacle: " + entity.name);
                var entityScript = entity.GetComponent<BaseEntity>();
                // We can get where characters are, other objects are not movable.
                if (entityScript is BaseCharacter || entityScript is MoneyEntity) continue;
                RemoveObstacleEdges(entity);
            }

            // Add cameras and other detectors to the model.
            foreach (GameObject entity in Entities)
            {
                var detectorEntity = entity.GetComponent<DetectorEntity>();
                if (detectorEntity == null) continue;
                detectorEntity.MarkDetectableNodes(AIModel, Tiles);
            }
        }

        private void RemoveObstacleEdges(GameObject entity)
        {
            var entityCollider = entity.GetComponent<Collider>();
            List<Collider> colliders = entity.GetComponentsInChildren<BoxCollider>().ToList<Collider>();
            colliders.Add(entityCollider);
            foreach (TileNode node in AIModel.Tiles)
            {
                foreach (TileEdge edge in node.Edges)
                {
                    Vector3 nodePosition = Tiles.Get(node.Position).transform.position;
                    IntegerTuple neighborIndices = edge.Neighbor.Position;
                    Vector3 neighborPosition = Tiles.Get(neighborIndices).transform.position;
                    Vector3 edgeCenter = (nodePosition + neighborPosition) / 2;
                    Vector3[] edgePoints = {nodePosition, edgeCenter, neighborPosition};
                    float distance = colliders.Min(collider =>
                    {
                        return edgePoints.Min(edgePoint =>
                        {
                            Vector3 closestPoint = collider.ClosestPoint(nodePosition);
                            return Vector3.Distance(edgePoint, closestPoint);
                        });
                    });
                    if (distance < MIN_OBSTACLE_DISTANCE)
                    {
                        edge.ObstructingEntities.Add(entity.GetComponent<BaseEntity>());
                    }
                }
            }
        }

        private TileEdge CanGetFromTo(TileNode start, int fromX, int fromY, int toX, int toY)
        {
            //Debug.Log("Can get from [" + fromX + ", " + fromY + "] to [" + toX + ", " + toY + "]?");
            if (toX < 0 || toY < 0 || toX >= Tiles.GetLength(0) || toY >= Tiles.GetLength(1))
            {
                return null;
            }

            GameObject fromObject = Tiles[fromX, fromY];
            GameObject toObject = Tiles[toX, toY];
            string fromName = fromObject.name;
            string toName = toObject.name;
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
                var fromIObstacle = fromObject.GetComponentInGameObjectOrChildren<IObstacle>();
                var toIObstacle = toObject.GetComponentInGameObjectOrChildren<IObstacle>();

                EdgeType fromType = fromIObstacle == null ? EdgeType.NONE : fromIObstacle.EdgeType;
                EdgeType toType = toIObstacle == null ? EdgeType.NONE : toIObstacle.EdgeType;
                EdgeType edgeType = EdgeType.NONE;
                IObstacle obstacle = null;
                if (fromObstacle && !fromType.Equals(EdgeType.NONE))
                {
                    edgeType = fromType;
                    obstacle = fromIObstacle;
                }

                if (toObstacle && !toType.Equals(EdgeType.NONE))
                {
                    edgeType = toType;
                    obstacle = toIObstacle;
                }

                return edgeType.Equals(EdgeType.NONE)
                    ? null
                    : new TileEdge(start, AIModel.Tiles[toX, toY], edgeType, 1, obstacle);
            }
            else
            {
                Vector3 fromTilePosition = Tiles[fromX, fromY].transform.position;
                Vector3 toTilePosition = Tiles[toX, toY].transform.position;
                fromTilePosition.y = 0.5f;
                toTilePosition.y = 0.5f;
                Ray fromToRay = new Ray(fromTilePosition, toTilePosition - fromTilePosition);
                RaycastHit[] hits = Physics.RaycastAll(fromToRay);
                float fromToDistance = Vector3.Distance(fromTilePosition, toTilePosition);
                if (hits.Any(hit => !hit.collider.name.Contains(GATE) && hit.distance <= fromToDistance))
                {
                    return null;
                }

                return new TileEdge(start, AIModel.Tiles[toX, toY], EdgeType.NORMAL, 1, null);
            }
        }

        public TileNode GetClosestTile(Vector3 position)
        {
            TileNode result = null;
            float minDistance = float.MaxValue;
            foreach (TileNode tile in AIModel.Tiles)
            {
                GameObject physicalTile = Tiles[tile.Position.First, tile.Position.Second];
                float potentialDistance = Vector3.Distance(position, physicalTile.transform.position);
                if (potentialDistance < minDistance)
                {
                    minDistance = potentialDistance;
                    result = tile;
                }
            }

            return result;
        }

        public void GetPlanningModel(BaseCharacter character, IntegerTuple goalCoords, GameObject finalObject,
            out PlanningNode startNode, out PlanningNode goalNode)
        {
            IEnumerable<GameObject> edgeCreatingEntities =
                Entities.Where(entity => entity.HasScriptOfType(typeof(IPlanningEdgeCreator)));
            Dictionary<IPlanningEdgeCreator, List<TileNode>> creatorsDictionary =
                BuildTileDictionary<IPlanningEdgeCreator>(edgeCreatingEntities);

            TileNode goalTileNode = AIModel.Tiles[goalCoords.First, goalCoords.Second];
            goalNode = new PlanningNode(goalTileNode, null, null, creatorsDictionary, character, finiteObject:finalObject);
            

            IEnumerable<EdgeType> currentItems =
                character.Items.Select(gameObject => gameObject.GetComponent<BaseItem>().CorrespondingEdgeType);
            TileNode startTileNode = GetClosestTile(character.transform.position);
            startNode = new PlanningNode(startTileNode, goalNode, currentItems.ToList(), creatorsDictionary, character, finiteObject: finalObject);
        }

        // Keys are the gameObjects, values the lists of the TileNodes that are close enough. 
        private Dictionary<T, List<TileNode>> BuildTileDictionary<T>(IEnumerable<GameObject> gameObjects)
        {
            var result = new Dictionary<T, List<TileNode>>();
            foreach (GameObject gameObject in gameObjects)
            {
                T component = gameObject.GetComponent<T>();
                foreach (TileNode tileNode in AIModel.Tiles)
                {
                    GameObject tileObject = Tiles[tileNode.Position.First, tileNode.Position.Second];
                    if (InteractHelper.AreCloseToInteract(tileObject, gameObject))
                    {
                        result.LazyAdd(component, tileNode);
                    }
                }
            }

            return result;
        }
    }
}
