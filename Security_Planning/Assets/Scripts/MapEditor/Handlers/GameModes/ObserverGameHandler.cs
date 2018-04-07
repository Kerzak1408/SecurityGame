using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using Assets.Scripts.MapEditor;
using UnityEngine;

public class ObserverGameHandler : BaseGameUserHandler
{
    private string name;

    public override string Name
    {
        get { return "Observer"; }
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Game.SwitchCamera();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Burglar.ChangePausedState();
        }
    }
}
