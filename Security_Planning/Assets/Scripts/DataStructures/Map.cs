using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
