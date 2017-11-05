using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiverEntity : BaseEntityWithBaseData
{
    public override void StartGame()
    {
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, position.y, -0.8f);
    }
}
