using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesHolder {

    private static ResourcesHolder instance;

    public Object[] AllTiles { get; private set; }
    public Object[] AllEntities { get; private set; }
    public UnityEngine.UI.Button MapButton { get; private set; }

    public const string PATH_PREFABS = "Prefabs/";
    public const string PATH_PREFABS_TILES = PATH_PREFABS + "Tiles/";
    public const string PATH_PREFABS_UI = PATH_PREFABS + "UI/";
    public const string PATH_PREFABS_ENTITIES = PATH_PREFABS + "Entities/";

    public static ResourcesHolder Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ResourcesHolder();
            }
            return instance;
        }
    }

    private ResourcesHolder()
    {
        AllTiles = Resources.LoadAll(PATH_PREFABS_TILES);
        AllEntities = Resources.LoadAll(PATH_PREFABS_ENTITIES);
        MapButton = Resources.Load<UnityEngine.UI.Button>(PATH_PREFABS_UI + "MapButton");
    }
}
