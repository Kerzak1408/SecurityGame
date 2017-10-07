using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {

    public GameObject[,] Tiles { get; private set; }
    public Dictionary<Tuple<int, int>, GameObject> Entities { get; private set; }
    public GameObject EmptyParent { get; private set; }

    public Map(GameObject[,] tiles, Dictionary<Tuple<int, int>, GameObject> entities, GameObject emptyParent)
    {
        Tiles = tiles;
        Entities = entities;
        EmptyParent = emptyParent;
    }

}
