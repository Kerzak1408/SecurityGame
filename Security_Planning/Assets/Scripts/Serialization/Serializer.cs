using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts.DataStructures;
using Assets.Scripts.Entities;
using Assets.Scripts.Model;
using UnityEngine;

namespace Assets.Scripts.Serialization
{
    public class Serializer
    {
        private static Serializer instance;
        public static Serializer Instance
        {
            get { return instance ?? (instance = new Serializer()); }
        }

        private BinaryFormatter binaryFormatter;
        private MemoryStream memoryStream;

        private Serializer()
        {
            binaryFormatter = new BinaryFormatter();
        }

        public byte[] Serialize(object serializedObject)
        {
            memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, serializedObject);
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }

        public byte[] SerializeGrid(GameObject[,] serializedGrid)
        {
            int height = serializedGrid.GetLength(0);
            int width = serializedGrid.GetLength(1);
            string[,] nameMatrix = new string[height, width];
            for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                nameMatrix[i, j] = serializedGrid[i, j].name;
            }
            return Serialize(nameMatrix);
        }

        public byte[] SerializeEntities(List<GameObject> serializedEntities)
        {
            var nameDictionary = new List<Tuple<string, BaseEntityData>>();
            foreach (GameObject entity in serializedEntities)
            {
                var baseEntity = entity.GetComponent<BaseEntity>();
                nameDictionary.Add(Tuple.New(baseEntity.PrefabName, baseEntity.Serialize()));
            }
            return Serialize(nameDictionary);
        }

        public T Deserialize<T>(string filename)
        {
            var fileStream = new FileStream(filename, FileMode.Open);
            var result = (T) binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            return result;
        }
    }
}
