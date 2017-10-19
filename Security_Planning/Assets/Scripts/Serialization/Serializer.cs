using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Serializer {
    private static Serializer instance;
    public static Serializer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Serializer();
            }
            return instance;
        }
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
        var nameDictionary = new List<Tuple<string, Vector3Wrapper>>();
        foreach (GameObject entity in serializedEntities)
        {
            nameDictionary.Add(Tuple.New<string, Vector3Wrapper>(entity.name, entity.transform.position));
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
