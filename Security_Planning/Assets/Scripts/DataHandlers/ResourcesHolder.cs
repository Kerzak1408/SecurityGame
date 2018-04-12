using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesHolder
{

    private static ResourcesHolder instance;

    public Object[] AllTiles { get; private set; }
    public Object[] AllEntities { get; private set; }
    public Object[] AllEntitiesIcons { get; private set; }
    public Object[] AllItems { get; private set; }
    public Object[] AllItemsIcons { get; private set; }
    public UnityEngine.UI.Button MapButton { get; private set; }
    public Texture2D CogwheelTexture;
    public Texture2D CogwheelPaleTaxture;
    public GameObject BehaviourToggle { get; private set; }
    public GameObject Vertex { get; private set; }
    public Object LineRenderer { get; private set; } 

    public static readonly string PATH_PREFABS = "Prefabs/";
    public static readonly string PATH_PREFABS_TILES = PATH_PREFABS + "Tiles/";
    public static readonly string PATH_PREFABS_UI = PATH_PREFABS + "UI/";
    public static readonly string PATH_PREFABS_ENTITIES = PATH_PREFABS + "Entities/";
    public static readonly string PATH_PREFABS_ENTITIESICONS = PATH_PREFABS + "EntitiesIcons/";
    public static readonly string PATH_PREFABS_ITEMS = PATH_PREFABS + "Items/";
    public static readonly string PATH_PREFABS_ITEMSICONS = PATH_PREFABS + "ItemsIcons/";
    public static readonly string PATH_IMAGES = "Images/";
    public static readonly string PATH_PREFABS_GRAPH = PATH_PREFABS + "Graph/";
    public static readonly string PATH_PREFABS_OTHER = PATH_PREFABS + "Other/";

    private static readonly string[] basicEntitiesNames = {"Burglar", "Guard"};

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

    public IEnumerable<Object> BasicEntities
    {
        get { return AllEntities.Where(entity => basicEntitiesNames.Contains(entity.name)); }
    }

    private ResourcesHolder()
    {
        AllTiles = Resources.LoadAll(PATH_PREFABS_TILES);
        AllEntities = Resources.LoadAll(PATH_PREFABS_ENTITIES);
        AllEntitiesIcons = Resources.LoadAll(PATH_PREFABS_ENTITIESICONS);
        MapButton = Resources.Load<UnityEngine.UI.Button>(PATH_PREFABS_UI + "MapButton");
        AllItems = Resources.LoadAll(PATH_PREFABS_ITEMS);
        AllItemsIcons = Resources.LoadAll(PATH_PREFABS_ITEMSICONS);
        CogwheelTexture = Resources.Load<Texture2D>(PATH_IMAGES + "Cogwheel");
        CogwheelPaleTaxture = Resources.Load<Texture2D>(PATH_IMAGES + "CogwheelPale");
        BehaviourToggle = Resources.Load<GameObject>(PATH_PREFABS_UI + "BehaviourToggle");
        Vertex = Resources.Load<GameObject>(PATH_PREFABS_GRAPH + "Vertex");
        LineRenderer = Resources.Load(PATH_PREFABS_OTHER + "LineRenderer");
    }
}
