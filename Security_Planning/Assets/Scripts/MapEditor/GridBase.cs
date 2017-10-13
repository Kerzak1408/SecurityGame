using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GridBase : MonoBehaviour {

    protected string MAPS_PATH;

    protected const string TILES = "Tiles";
    protected const string ENTITIES = "Entities";
    protected const string PASSWORDS = "Passwords";

    // Use this for initialization
    protected virtual void Start () {
        MAPS_PATH = FileHelper.JoinPath(Application.persistentDataPath, "Maps");
    }

    // Update is called once per frame
    void Update () {
		
	}

    protected virtual Map LoadMap(string mapName, UnityEngine.UI.Button correspondingButton = null, bool mapVisible = false)
    {
        mapName =  mapName.Replace(' ', '_');
        var serializer = Serializer.Instance;
        var namesMatrix = serializer.Deserialize<string[,]>(FileHelper.JoinPath(MAPS_PATH, mapName, TILES));
        var namesDictionary = serializer.Deserialize<Dictionary<Tuple<int, int>, string>>(FileHelper.JoinPath(MAPS_PATH, mapName, ENTITIES));
        var passwordDictionary = serializer.Deserialize<Dictionary<Tuple<int, int>, string>>(FileHelper.JoinPath(MAPS_PATH, mapName, PASSWORDS));
        var allTiles = ResourcesHolder.Instance.AllTiles;

        foreach (System.Collections.Generic.KeyValuePair<Tuple<int, int>, string> kvPair in passwordDictionary)
        {
            Debug.Log("KEY=" + kvPair.Key + " VALUE=" + kvPair.Value);
        }

        int width = namesMatrix.GetLength(1);
        int height = namesMatrix.GetLength(0);
        var loadedGrid = new GameObject[height, width];
        GameObject emptyParent = new GameObject();
        emptyParent.SetActive(mapVisible);
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
            {
                string currentName = namesMatrix[i, j];
                var newTile = allTiles.FindByName(currentName);
                GameObject newObject = Instantiate(newTile, transform) as GameObject;
                loadedGrid[i, j] = newObject;
                newObject.transform.position = new Vector3(j - width / 2, i - height / 2, 0);
                newObject.name = newTile.name;
                newObject.transform.parent = emptyParent.transform;
                if (newObject.HasScriptOfType<PasswordGate>())
                {
                    var passwordGateScript = newObject.GetComponent<PasswordGate>();
                    var password = passwordDictionary[Tuple.New(i, j)];
                    passwordGateScript.Password = password;
                }
            }

        var allEntities = ResourcesHolder.Instance.AllEntities;
        var dictionary = new Dictionary<Tuple<int, int>, GameObject>();
        foreach (KeyValuePair<Tuple<int, int>, string> kvPair in namesDictionary)
        {
            var currentName = kvPair.Value;
            var newEntity = allEntities.FindByName(currentName);
            GameObject newObject = Instantiate(newEntity, transform) as GameObject;
            newObject.name = newEntity.name;
            newObject.transform.position = new Vector3(kvPair.Key.Second - width / 2, kvPair.Key.First - height / 2, -1);
            newObject.transform.parent = emptyParent.transform;
            dictionary.Add(kvPair.Key, newObject);
        }

        return new Map(loadedGrid, dictionary, emptyParent, passwordDictionary);
    }    
   
}
