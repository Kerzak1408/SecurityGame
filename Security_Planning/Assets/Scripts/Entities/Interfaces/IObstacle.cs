using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Model;
using UnityEngine;

public interface IObstacle
{
    EdgeType EdgeType { get; }
    IInteractable InteractableObject { get; }
}
