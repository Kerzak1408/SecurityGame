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
        //map.EmptyParent.transform.Rotate(90, 0, 0);
        //map.Entities.Values.Find();
        var guard = GameObject.Find("Guard");
        guard.transform.Rotate(-90, 0, 0);
        var guardPosition = guard.transform.position;
        var cameraPosition = Camera.main.transform.position;
        Camera.main.transform.position = guardPosition;
        Camera.main.orthographicSize = 1;
        Camera.main.transform.parent = guard.transform;
        Camera.main.transform.localRotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
