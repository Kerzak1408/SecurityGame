using System.Collections.Generic;
using Assets.Scripts.Extensions;
using UnityEngine;

namespace Assets.Scripts.DataStructures
{
    public class Map {

        public GameObject[,] Tiles { get; private set; }
        public List<GameObject> Entities { get; private set; }
        public Dictionary<Tuple<int, int>, string> PasswordDictionary { get; private set; }
        public GameObject EmptyParent { get; private set; }

        public Map(GameObject[,] tiles, List<GameObject> entities, GameObject emptyParent, Dictionary<Tuple<int, int>, string> passwordDictionary)
        {
            Tiles = tiles;
            Entities = entities;
            EmptyParent = emptyParent;
            PasswordDictionary = passwordDictionary;
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
        public void DeactivateEntitiesExceptOfType<T>() where T : MonoBehaviour
        {
            foreach (GameObject entity in Entities)
            {
                bool activate = entity.HasScriptOfType<T>();
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
    }
}
