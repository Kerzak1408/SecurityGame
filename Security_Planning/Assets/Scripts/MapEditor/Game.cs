using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : GridBase
{

    private Map map;

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();
        string mapName = Scenes.GetParam("map");
        map = LoadMap(mapName, mapVisible:true);
        //map.Entities.Values.Find();
        var guard = GameObject.Find("Guard");
        var guardPosition = guard.transform.position;
        var cameraPosition = Camera.main.transform.position;
        Camera.main.transform.position = new Vector3(guardPosition.x, guardPosition.y, cameraPosition.z);
        Camera.main.orthographicSize = 1;
        Camera.main.transform.parent = guard.transform;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
