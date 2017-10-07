using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : GridBase {

	// Use this for initialization
	protected override void Start () {
        base.Start();
        string mapName = Scenes.GetParam("map");
        LoadMap(mapName, mapVisible:true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
