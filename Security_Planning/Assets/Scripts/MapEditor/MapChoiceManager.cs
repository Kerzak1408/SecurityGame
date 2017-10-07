using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapChoiceManager : GridsBrowserBase {

    protected override void HoverEnded()
    {
    }

    protected override void HoverLogic(Ray ray)
    {
    }

    protected override void LeftButtonUpLogicNormalPhase(Ray ray)
    {
    }

    protected override void RightButtonUpLogicNormalPhase(Ray ray)
    {

    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public void LoadGame()
    {
        Scenes.Load(Scenes.MAIN_SCENE, "map", SelectedMapButton.GetComponentInChildren<Text>().text);   
    }
}
